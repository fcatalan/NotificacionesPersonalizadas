using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Linq.Expressions;

namespace Minvu.Notificaciones.IData.ORM
{
	public class ContextoBD : NotificacionesEntities
	{
		public ContextoBD() : base()
		{
			try
			{
				this.Database.CommandTimeout = Convert.ToInt32(ConfigurationManager.AppSettings["TimeoutComandoBD"]);
			}
			catch (Exception ex)
			{
				this.Database.CommandTimeout = 60;
			}
		}

		public override int SaveChanges()
		{
			int result = -1;
			try
			{
				result = base.SaveChanges();
			}
			catch (DbEntityValidationException e)
			{
				StringBuilder errores = new StringBuilder();
				foreach (var eve in e.EntityValidationErrors)
				{

					errores.Append(string.Format("Entidad de tipo \"{0}\" en estado \"{1}\" posee los siguientes errores de validación:\n",
							eve.Entry.Entity.GetType().Name, eve.Entry.State));
					foreach (var ve in eve.ValidationErrors)
					{
						errores.Append(string.Format("- Propiedad: \"{0}\", Error: \"{1}\"\n",
								ve.PropertyName, ve.ErrorMessage));
					}
				}
				DbEntityValidationException dbe = new DbEntityValidationException(errores.ToString());
				throw dbe;
			}
			catch (Exception ex)
			{
				Log.Log.RegistrarError(ex, ex.Message);
			}
			return result;
		}

		public static int GetMaxLength<TEntity>(Expression<Func<TEntity, string>> property)
		{
			ObjectContext oc = ((IObjectContextAdapter)new ContextoBD()).ObjectContext;
			var test = oc.MetadataWorkspace.GetItems(DataSpace.CSpace);

			if (test == null)
				return -1;

			Type entType = typeof(TEntity);
			string propertyName = ((MemberExpression)property.Body).Member.Name;

			var q = test
					.Where(m => m.BuiltInTypeKind == BuiltInTypeKind.EntityType)
					.SelectMany(meta => ((EntityType)meta).Properties
					.Where(p => p.Name == propertyName && p.TypeUsage.EdmType.Name == "String"));

			var queryResult = q.Where(p =>
			{
				var match = p.DeclaringType.Name == entType.Name;
				if (!match)
					match = entType.Name == p.DeclaringType.Name;

				return match;

			})
					.Select(sel => sel.TypeUsage.Facets["MaxLength"].Value)
					.ToList();

			if (queryResult.Any())
			{
				int result = Convert.ToInt32(queryResult.First());
				return result;
			}
			return -1;
		}
		/*
		public static int? GetMaxLength(this EntityObject entityObject, string entityProperty)
		{
			ContextoBD _context = new ContextoBD();
			int? result = null;
			using (_context)
			{
				var q = from meta in ((IObjectContextAdapter)_context).ObjectContext.MetadataWorkspace.GetItems(DataSpace.CSpace)
														.Where(m => m.BuiltInTypeKind == BuiltInTypeKind.EntityType)
								from p in (meta as EntityType).Properties
										.Where(p => p.DeclaringType.Name == entityObject.GetType().Name
												&& p.Name == entityProperty
												&& p.TypeUsage.EdmType.Name == "String")
								select p;

				var queryResult = from meta in ((IObjectContextAdapter)_context).ObjectContext.MetadataWorkspace.GetItems(DataSpace.CSpace)
														.Where(m => m.BuiltInTypeKind == BuiltInTypeKind.EntityType)
													from p in (meta as EntityType).Properties
															.Where(p => p.DeclaringType.Name == entityObject.GetType().Name
																	&& p.Name == entityProperty
																	&& p.TypeUsage.EdmType.Name == "String")
													select p.TypeUsage.Facets["MaxLength"].Value;
				if (queryResult.Count() > 0)
				{
					result = Convert.ToInt32(queryResult.First());
				}
			}
			return result;
		}
		*/
	}
}
