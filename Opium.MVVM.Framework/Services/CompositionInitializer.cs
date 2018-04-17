// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------

using System.Composition.Hosting;

namespace System.ComponentModel.Composition
{
    using System;
    using System.ComponentModel.Composition.Hosting;
    using System.Globalization;
    using System.Linq;

    /// <summary>
    /// A static class used to satisfy the imports on a object instance based on a <see cref="CompositionContainer"/>
    ///     registered with the <see cref="CompositionHost"/>. 
    /// </summary>
    public static class CompositionInitializer
    {
        /// <summary>
        /// Delegate used to create the composition globalContainer
        /// </summary>
        private static Func<CompositionContainer> createContainer = CreateCompositionContainer;

        /// <summary>
        ///     Will satisfy the imports on a object instance based on a <see cref="CompositionContainer"/>
        ///     registered with the <see cref="CompositionHost"/>. By default if no <see cref="CompositionContainer"/>
        ///     is registered the first time this is called it will be initialized to a catalog
        ///     that contains all the assemblies in the entry assembly folder, plus, optionally, an Extensions folder.
        /// </summary>
        /// <param name="attributedPart">
        ///     Object instance that contains <see cref="ImportAttribute"/>s that need to be satisfied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="attributedPart"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     <paramref name="attributedPart"/> contains <see cref="ExportAttribute"/>s applied on its type.
        /// </exception>
        /// <exception cref="ChangeRejectedException">
        ///     One or more of the imports on the object instance could not be satisfied.
        /// </exception>
        /// <exception cref="CompositionException">
        ///     One or more of the imports on the object instance caused an error while composing.
        /// </exception>
        public static void SatisfyImports(object attributedPart)
        {
            if (attributedPart == null)
            {
                throw new ArgumentNullException("attributedPart");
            }

            var batch = new CompositionBatch();

            var part = batch.AddPart(attributedPart);

            if (part.ExportDefinitions.Any())
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Strings.ArgumentException_TypeHasExports, attributedPart.GetType().FullName), "attributedPart");
            }

            CompositionContainer container;

            // Ignoring return value because we don't need to know if we created it or not
            CompositionHost.TryGetOrCreateContainer(createContainer, out container);

            container.Compose(batch);
        }

        /// <summary>
        /// Creates the composition globalContainer.
        /// </summary>
        /// <returns>The new CompositionContainer</returns>
        private static CompositionContainer CreateCompositionContainer()
        {
            return new CompositionContainer(CompositionHost.CreateDefaultCatalog());
        }
    }
}