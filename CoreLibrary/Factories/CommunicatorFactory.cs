using CoreLibrary.Interfaces;
using CoreLibrary.Utilities;

namespace CoreLibrary.Factories
{
    public static class CommunicatorFactory
    {
        /// <summary>
        /// Returns the type specified in our config file as a Communicator.
        /// We currently have TCP and UDP protocols implemented.
        /// If implementing new protocols, make sure to define them in CoreLibrary.Communication namespace
        /// (any subfolder, if needed, will do)
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static ICommunicator Create(Configuration config)
        {
            var communicatorType = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .FirstOrDefault(selectedType =>
                    selectedType.IsClass &&
                    !selectedType.IsAbstract &&
                    typeof(ICommunicator).IsAssignableFrom(selectedType) &&
                    selectedType.Name.Equals(config.Communicator, StringComparison.OrdinalIgnoreCase) &&
                    selectedType.Namespace != null &&
                    selectedType.Namespace.Contains("CoreLibrary.Communication")); // Make sure that all new communication type implementations go to CoreLibrary.Communication namespace

            if (communicatorType == null)
                throw new ArgumentException($"Communicator type '{config.Communicator}' not found.");

            try
            {
                var instance = Activator.CreateInstance(communicatorType, config);
                return instance as ICommunicator
                    ?? throw new InvalidOperationException($"Unable to cast {config.Communicator} to ICommunicator.");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to create communicator: {ex.Message}", ex);
            }
        }
    }
}
