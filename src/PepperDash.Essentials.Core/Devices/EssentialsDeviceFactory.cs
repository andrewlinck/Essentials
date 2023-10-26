﻿using System.Collections.Generic;
using PepperDash.Essentials.Core.Config;

namespace PepperDash.Essentials.Core
{
    /// <summary>
    /// Devices the basic needs for a Device Factory
    /// </summary>
    public abstract class EssentialsDeviceFactory<T> : IDeviceFactory where T:EssentialsDevice
    {
        #region IDeviceFactory Members

        /// <summary>
        /// A list of strings that can be used in the type property of a DeviceConfig object to build an instance of this device
        /// </summary>
        public List<string> TypeNames { get; protected set; }

        /// <summary>
        /// Loads an item to the DeviceFactory.FactoryMethods dictionary for each entry in the TypeNames list
        /// </summary>
        public void LoadTypeFactories()
        {
            foreach (var typeName in TypeNames)
            {
                //Debug.Console(2, "Getting Description Attribute from class: '{0}'", typeof(T).FullName);
                var    descriptionAttribute = typeof(T).GetCustomAttributes(typeof(DescriptionAttribute), true) as DescriptionAttribute[];
                string description          = descriptionAttribute[0].Description;
                var    snippetAttribute     = typeof(T).GetCustomAttributes(typeof(ConfigSnippetAttribute), true) as ConfigSnippetAttribute[];
                DeviceFactory.AddFactoryForType(typeName.ToLower(), description, typeof(T), BuildDevice);
            }
        }

        /// <summary>
        /// The method that will build the device
        /// </summary>
        /// <param name="dc">The device config</param>
        /// <returns>An instance of the device</returns>
        public abstract EssentialsDevice BuildDevice(DeviceConfig dc);

        #endregion
    }
}