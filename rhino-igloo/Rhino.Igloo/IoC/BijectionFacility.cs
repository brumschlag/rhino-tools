﻿#region license
// Copyright (c) 2005 - 2007 Ayende Rahien (ayende@ayende.com)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Ayende Rahien nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion


using System;
using System.Collections.Generic;
using System.Reflection;
using Castle.ActiveRecord;
using Castle.MicroKernel.Facilities;
using Rhino.Igloo;
using log4net;
using Rhino.Commons;

namespace Rhino.Igloo
{
    /// <summary>
    /// Setup the container so we can get Bijection support for controllers and views
    /// </summary>
    public class BijectionFacility : AbstractFacility
    {
        private static ILog logger = LogManager.GetLogger(typeof(BijectionFacility));

        private readonly Assembly[] assemblies;

        /// <summary>
        /// Initializes a new instance of the <see cref="BijectionFacility"/> class.
        /// </summary>
        /// <param myName="assemblies">The assemblies.</param>
        public BijectionFacility(params Assembly[] assemblies)
        {
            this.assemblies = assemblies;
        }

        /// <summary>
        /// The custom initialization for the Facility.
        /// </summary>
        /// <remarks>It must be overriden.</remarks>
        protected override void Init()
        {
            // Added a ComponentCache Repository to track it
            Kernel.AddComponent("component.repository", typeof(ComponentRepository));
            Kernel.ComponentModelBuilder.AddContributor(new InjectionInspector());
            Kernel.ComponentCreated += new Castle.MicroKernel.ComponentInstanceDelegate(Kernel_ComponentCreated);
            RegisterViewComponent();
            RegisterControllers();
        }

        private void RegisterControllers()
        {
            Kernel.AddComponent("BaseController", typeof(BaseController));
            foreach (Assembly assembly in assemblies)
            {
                foreach (Type type in assembly.GetExportedTypes())
                {
                    if (typeof(BaseController).IsAssignableFrom(type) &&
                        AttributeUtil.ShouldSkipAutomaticRegistration(type)==false)
                    {
                        Kernel.AddComponent(type.Name, type);
                    }
                }
            }
        }

        /// <summary>
        /// Registers the view component.
        /// </summary>
        private void RegisterViewComponent()
        {
            ComponentRepository repository = (ComponentRepository)Kernel[typeof(ComponentRepository)];

            foreach (Assembly assembly in assemblies)
            {
                foreach (Type type in assembly.GetExportedTypes())
                {
                    // MasterPage / Web page / UserControl
                    if (AttributeUtil.IsView(type))
                    {
                        repository.AddComponent(type);
                    }
                }
            }
        }

        private static void Kernel_ComponentCreated(Castle.Core.ComponentModel model, object instance)
        {
            IDictionary<InjectAttribute, PropertyInfo> members =
                model.ExtendedProperties[InjectionInspector.InMembers] as IDictionary<InjectAttribute, PropertyInfo>;
            IDictionary<InjectEntityAttribute, PropertyInfo> entitiesToInject =
                model.ExtendedProperties[InjectionInspector.InEntityMembers] as IDictionary<InjectEntityAttribute, PropertyInfo>;
            if (members != null)
                InjectMembers(instance, members);
            if (entitiesToInject != null)
                InjectEntities(instance, entitiesToInject);
        }


        /// <summary>
        /// Injects the members.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="membersToInject">The members to inject.</param>
        private static void InjectMembers(Object instance,
            IDictionary<InjectAttribute, PropertyInfo> membersToInject)
        {
            if (membersToInject != null && membersToInject.Count > 0)
            {
                foreach (KeyValuePair<InjectAttribute, PropertyInfo> kvp in membersToInject)
                {
                    PropertyInfo propertyInfo = kvp.Value;
                    string instanceToInject = Scope.Input[kvp.Key.Name];

                    if (instanceToInject == null)
                        continue;
                    try
                    {

                        object result = ConversionUtil.ConvertTo(propertyInfo.PropertyType, instanceToInject);
						if (result != null)
							propertyInfo.SetValue(instance, result, null);
                    }
                    catch(Exception e)
                    {
                        logger.Debug(string.Format("Failed to convert {0} to type {1}", kvp.Key.Name, kvp.Value.PropertyType), e);
                    }
                }
            }
        }

        private static void InjectEntities(object instance, IDictionary<InjectEntityAttribute, PropertyInfo> entitiesToInject)
        {
            foreach (KeyValuePair<InjectEntityAttribute, PropertyInfo> kvp in entitiesToInject)
            {
                object key = ConvertKey(instance, Scope.Input[kvp.Key.Name], kvp);
                if (key == null)
                    continue;

                object entity = ActiveRecordMediator.FindByPrimaryKey(kvp.Value.PropertyType, key, false);
                kvp.Value.SetValue(instance, entity, null);
            }
        }

        private static object ConvertKey(object instance, string key, KeyValuePair<InjectEntityAttribute, PropertyInfo> kvp)
        {
            if (key == null)
                return null;
            try
            {
                return ConversionUtil.ConvertTo(typeof(int), key);;
            }
            catch (Exception e)
            {
                logger.Debug(string.Format("Failed to convert ID ({0}) for {1} to integer.", kvp.Key.Name, instance), e);
                return null;
            }
        }
    }


}
