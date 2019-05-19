using System;
using System.Collections.Generic;
using System.Linq;
using LMPT.Core.Server.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

public class SideBarHelper  
{
    private readonly IServiceProvider serviceProvider;

    public SideBarHelper(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }
    
    public  List<ISideBarViewModel> GetAllSidebars()
    {
        var sidebarTypes = System.Reflection.Assembly.GetAssembly(this.GetType()).GetTypes()
            .Where(x => x.GetInterfaces().Any(t => t == typeof(ISideBarViewModel)));

        return sidebarTypes.Select(sbt =>
            (ISideBarViewModel)this.serviceProvider.GetService(sbt)).ToList();
            
    }
}