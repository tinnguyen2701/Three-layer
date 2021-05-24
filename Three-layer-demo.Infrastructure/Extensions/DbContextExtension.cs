using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Three_layer_demo.Infrastructure.Extensions
{
    public static class DbContextExtension
    {
        private static readonly object ConnectionLocked = new object();

        public static DbCommand LoadStoredProc(this DbContext context, string storedProcName, bool prependDefaultSchema = true)
        {
            var conn = context.Database.GetDbConnection();
            var cmd = conn.CreateCommand();

            if (prependDefaultSchema)
            {
                var schemaName = context.Model.GetDefaultSchema();

                if (schemaName != null)
                {
                    storedProcName = $"{schemaName}.{storedProcName}";
                }
            }

            if (context.Database.CurrentTransaction != null)
            {
                cmd.Transaction = context.Database.CurrentTransaction.GetDbTransaction();

            }

            cmd.CommandText = storedProcName;
            cmd.CommandType = CommandType.StoredProcedure;

            return cmd;
        }

		public static async Task ExecuteStoredProcAsync(this DbCommand command, Action<SprocResults> handleResults, CommandBehavior commandBehaviour = CommandBehavior.Default, CancellationToken ct = default, bool manageConnection = false)
		{
			if (handleResults == null)
			{
				throw new ArgumentNullException(nameof(handleResults));
			}

			using (command)
			{
				lock (ConnectionLocked)
				{
					if (command.Connection.State == ConnectionState.Closed)
					{
						command.Connection.Open();
					}
				}
				try
				{
					using (var reader = await command.ExecuteReaderAsync(commandBehaviour, ct).ConfigureAwait(false))
					{
						var results = new SprocResults(reader);
						handleResults(results);
					}
				}
				finally
				{
					if (manageConnection)
					{
						command.Connection.Close();
					}
					command.Dispose();
				}
			}
		}

		public class SprocResults
		{

			//  private DbCommand _command;
			private DbDataReader _reader;

			public SprocResults(DbDataReader reader)
			{
				// _command = command;
				_reader = reader;
			}

			public List<T> ReadToList<T>()
			{
				return MapToList<T>(_reader);
			}

			public List<T> ReadNextListOrEmpty<T>()
			{
				var items = ReadToList<T>();

				_reader.NextResult();

				return items ?? new List<T>();
			}

			public T? ReadToValue<T>() where T : struct
			{
				return MapToValue<T>(_reader);
			}

			public Task<bool> NextResultAsync()
			{
				return _reader.NextResultAsync();
			}

			public Task<bool> NextResultAsync(CancellationToken ct)
			{
				return _reader.NextResultAsync(ct);
			}

			public bool NextResult()
			{
				return _reader.NextResult();
			}

			/// <summary>
			/// Retrieves the column values from the stored procedure and maps them to <typeparamref name="T"/>'s properties
			/// </summary>
			/// <typeparam name="T"></typeparam>
			/// <param name="dr"></param>
			/// <returns>List<<typeparamref name="T"/>></returns>
			private List<T> MapToList<T>(DbDataReader dr)
			{
				var objList = new List<T>();
				var props = typeof(T).GetRuntimeProperties().ToList();

				var colMapping = dr.GetColumnSchema()
					.Where(x => props.Any(y => y.Name.ToLower() == x.ColumnName.ToLower()))
					.ToDictionary(key => key.ColumnName.ToLower());

				if (dr.HasRows)
				{
					while (dr.Read())
					{
						T obj = Activator.CreateInstance<T>();
						foreach (var prop in props)
						{
							if (colMapping.ContainsKey(prop.Name.ToLower()))
							{
								var column = colMapping[prop.Name.ToLower()];

								if (column?.ColumnOrdinal != null)
								{
									var val = dr.GetValue(column.ColumnOrdinal.Value);
									if (prop.CanWrite)
									{
										prop.SetValue(obj, val == DBNull.Value ? null : val);
									}
								}

							}
						}
						objList.Add(obj);
					}
				}
				return objList;
			}

			/// <summary>
			///Attempts to read the first value of the first row of the result set.
			/// </summary>
			private T? MapToValue<T>(DbDataReader dr) where T : struct
			{
				if (dr.HasRows)
				{
					if (dr.Read())
					{
						return dr.IsDBNull(0) ? new T?() : dr.GetFieldValue<T>(0);
					}
				}
				return new T?();
			}
		}

	}
}
