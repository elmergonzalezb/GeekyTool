﻿using Autofac;
using GeekyTool.Samples.ViewModels;

namespace GeekyTool.Samples
{
    public class ViewModelLocator
    {
        private readonly IContainer container;

        public ViewModelLocator()
        {
            var builder = new ContainerBuilder();

            // Interfaces

            // ViewModels
            builder.RegisterType<ShellViewModel>();
            builder.RegisterType<MainViewModel>();
            builder.RegisterType<DominantColorViewModel>();

            container = builder.Build();
        }

        public ShellViewModel ShellViewModel => container.Resolve<ShellViewModel>();
        public MainViewModel MainViewModel => container.Resolve<MainViewModel>();
        public DominantColorViewModel DominantColorViewModel => container.Resolve<DominantColorViewModel>();
    }
}
