using Repository.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Data.Entity;

namespace Repository.Repository
{
    public class Repository<TModel, TViewModel> : IRepository<TModel, TViewModel> where TModel : class where TViewModel : IViewModel<TModel>, new()
    {

        private DbContext context;

        protected virtual DbSet<TModel> DbSet { get { return context.Set<TModel>(); } }

        public Repository(DbContext context)
        {
            this.context = context;
        }


        public virtual ICollection<TViewModel> Get()
        {
            var data = new List<TViewModel>();
            foreach (var model in DbSet)
            {
                TViewModel obj = new TViewModel();
                obj.FromDataBase(model);
                data.Add(obj);
            }
            return data;
        }

        public virtual TViewModel Get(params string[] keys)
        {
            return GetTViewModelByParams(keys); ;
        }

        public virtual ICollection<TViewModel> Get(Expression<Func<TModel, bool>> query)
        {
            var dataQuery = DbSet.Where(query);
            var data = new List<TViewModel>();

            foreach (var model in dataQuery)
            {
                TViewModel obj = new TViewModel();
                obj.FromDataBase(model);
                data.Add(obj);
            }
            return data;
        }

        public virtual TViewModel Get(params int[] keys)
        {
            return GetTViewModelByParams(keys);
        }

        public virtual TViewModel Get(params Guid[] keys)
        {
            return GetTViewModelByParams(keys);
        }

        public virtual TViewModel Get(params object[] keys)
        {
            return GetTViewModelByParams(keys);
        }
        public virtual TViewModel Add(TViewModel model)
        {
            var mdl = model.ToDataBase();
            DbSet.Add(mdl);
            try
            {
                context.SaveChanges();
                model.FromDataBase(mdl);
                return model;
            }
            catch (Exception e)
            {
                return default(TViewModel);
            }
        }

        public virtual int Delete(Expression<Func<TModel, bool>> query)
        {
            var data = DbSet.Where(query);
            DbSet.RemoveRange(data);

            try
            {
                return context.SaveChanges();
            }
            catch (Exception e)
            {
                return 0;
            }
        }

        public virtual int Delete(TViewModel model)
        {
            return UpdateDeleteMethod(model, "Delete");     
        }

        

        public virtual int Update(TViewModel model)
        {
            return UpdateDeleteMethod(model, "Update");
        }

        private TViewModel GetTViewModelByParams(params dynamic[] keys)
        {
            var data = DbSet.Find(keys);
            var result = new TViewModel();
            result.FromDataBase(data);
            return result;
        }

        private int UpdateDeleteMethod(TViewModel model, string action)
        {
            var keysInt = model.GetKeysInt();
            var keysString = model.GetKeysString();
            var keysObj = model.GetKeysObject();
            var keysGuid = model.GetKeysGuid();

            dynamic obj;

            if (keysInt.Any())
            {
                obj = DbSet.Find(keysInt);
            }
            else if (keysString.Any())
            {
                obj = DbSet.Find(keysString);
            }
            else if (keysGuid.Any())
            {
                obj = DbSet.Find(keysGuid);
            }
            else
            {
                obj = DbSet.Find(keysObj);
            }

            if(action == "Update")
            {
                model.UpdateDataBase(obj);
            }
            else
            {
                DbSet.Remove(obj);
            }
            

            try
            {
                return context.SaveChanges();
            }
            catch (Exception e)
            {

                return 0;
            }
        }
    }
}
