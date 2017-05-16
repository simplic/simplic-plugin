using System;

namespace Simplic.Plugin
{
    /// <summary>
    /// A simple plugin interface   
    /// </summary>
    public interface IPlugin : IDisposable
    {
        /// <summary>
        /// Initialization method for the plugin.
        /// </summary>
        void Init();

        /// <summary>
        /// Entry point of the plugin.
        /// </summary>
        void Run();

        /// <summary>
        /// This is the installation entry point for the plugins.
        /// </summary>
        void Setup(Version oldVersion);        

        /// <summary>
        /// The name of the plugin.
        /// </summary>
        string PluginName { get; }

        /// <summary>
        /// The plugin version
        /// </summary>
        Version PluginVersion { get; }

        /// <summary>
        /// The Id of the plugin.
        /// </summary>
        Guid PluginId { get; }
        
    }
}
