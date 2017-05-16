using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;

namespace Simplic.Plugin
{
    /// <summary>
    /// Simplic plugin host.
    /// </summary>
    public class PluginHost
    {
        private CompositionContainer compositionContainer;
        private AggregateCatalog aggregateCatalog; // An aggregate catalog that combines multiple catalogs
        private bool isComposed;

        public bool IsComposed
        {
            get
            {
                return isComposed;
            }

            private set
            {
                isComposed = value;
            }
        }

        /// <summary>
        /// This constructor takes parameters, creates a MEF catalog and looks for dll files in <paramref name="pluginDirectory"/>
        /// matching the <paramref name="searchPattern"/>.
        /// </summary>
        /// <param name="pluginDirectory">The directory to look for the plugins</param>
        /// <param name="searchPattern">A search pattern to filter out plugin dlls.</param>
        /// <param name="initLater">If this is set, the plugins Init() method wont be called imidiately.</param>
        public PluginHost(string pluginDirectory, string searchPattern)
        {
            if (string.IsNullOrWhiteSpace(pluginDirectory))
                throw new System.ArgumentNullException(nameof(pluginDirectory));


            aggregateCatalog = new AggregateCatalog();

            //Adds all the parts found in the same assembly as the Program class
            aggregateCatalog.Catalogs.Add(new DirectoryCatalog(pluginDirectory, searchPattern));
            

            //Create the CompositionContainer with the parts in the catalog
            compositionContainer = new CompositionContainer(aggregateCatalog);            
        }

        public void ComposeParts()
        {
            //Fill the imports of this object
            try
            {
                compositionContainer.ComposeParts(this);

                IsComposed = true;
            }
            catch (CompositionException compositionException)
            {
                //TODO: Log exception
                throw;
            }
        }

        public bool AddAssembly(string fileName)
        {
            // TODO: Decide if we really need to break the whole system and throw an exception or just log it and ignore it.
            if (!File.Exists(fileName))
                throw new FileNotFoundException(fileName + " was not found.");

            // if the parts ( plugins ) are already composed, we cant add any assembly
            if (IsComposed)
                return false;

            aggregateCatalog.Catalogs.Add(new AssemblyCatalog(fileName));

            return true;
        }

        /// <summary>
        /// Adds a directory to be loaded later
        /// </summary>
        /// <param name="newDirectory"></param>
        /// <param name="searchPattern"></param>
        public bool AddDirectory(string newDirectory, string searchPattern)
        {
            // if the parts ( plugins ) are already composed, we cant add any assembly
            if (IsComposed)
                return false;

            //Adds all the parts found in the same assembly as the Program class
            aggregateCatalog.Catalogs.Add(new DirectoryCatalog(newDirectory, searchPattern));

            return true;
        }

        /// <summary>
        /// Searches for a plugin with the given ID and returns it.
        /// </summary>
        /// <param name="pluginName">Plugin ID to look for</param>
        /// <returns>A Plugin</returns>
        public IPlugin GetPluginById(Guid pluginId)
        {
            foreach (var item in compositionContainer.GetExportedValues<IPlugin>())
            {
                if (item.PluginId == pluginId)
                    return item;
            }

            throw new System.ArgumentOutOfRangeException("Plugin does not exist.");
        }

        /// <summary>
        /// Searches for a plugin with the given name and returns it.
        /// </summary>
        /// <param name="pluginName">Plugin name to look for</param>
        /// <returns>A Plugin</returns>
        public IPlugin GetPluginByName(string pluginName)
        {
            foreach (var item in compositionContainer.GetExportedValues<IPlugin>())
            {
                if (item.PluginName.ToLower() == pluginName.ToLower())
                    return item;
            }

            throw new System.ArgumentOutOfRangeException("Plugin does not exist.");
        }

        /// <summary>
        /// This method would iterate through all the loaded plugins and call their Init() method.
        /// </summary>
        public void InitAll()
        {
            foreach (var item in compositionContainer.GetExportedValues<IPlugin>())
            {
                item.Init();
            }
        }

        /// <summary>
        /// This method would iterate through all the loaded plugins and call their Run() method.
        /// </summary>
        public void RunAll()
        {
            foreach (var item in compositionContainer.GetExportedValues<IPlugin>())
            {
                item.Run();
            }
        }
    }
}