﻿// <copyright file="ServerSelectWindowViewModel.cs" company="TThread">
// Copyright (c) TThread. All rights reserved.
// </copyright>

namespace TTServerMaker.Engine.ViewModels.ServerSelect;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using TTServerMaker.Engine.Models.Servers;
using TTServerMaker.Engine.Services;

/// <summary>
/// The view model of selecting a server.
/// </summary>
public class ServerSelectViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ServerSelectViewModel"/> class.
    /// </summary>
    public ServerSelectViewModel()
    {
        this.ServerInfoManager = Ioc.Default.GetService<IBasicInfoManagerService>();

        this.LoadServerCommand = new RelayCommand<BasicInfo>((parameter) =>
        {
            this.SelectedServer = parameter;
        });

        this.LoadServers();
    }

    /// <summary>
    /// Gets the view model for adding a server.
    /// </summary>
    public AddServerViewModel AddServerVM { get; }

    /// <summary>
    /// Gets the command run when a server is about to loaded.
    /// </summary>
    public ICommand LoadServerCommand { get; }

    /// <summary>
    /// Gets the server info the user selected to load up.
    /// </summary>
    public BasicInfo SelectedServer { get; private set; }

    /// <summary>
    /// Gets or sets the list of the servers.
    /// </summary>
    public ObservableCollection<BasicInfo> ServerInfoList { get; set; }

    private IBasicInfoManagerService ServerInfoManager { get; }

    private void LoadServers()
    {
        Task<ObservableCollection<BasicInfo>> task = new Task<ObservableCollection<BasicInfo>>(() =>
        new ObservableCollection<BasicInfo>(this.ServerInfoManager.GetServerInfos()));

        task.ContinueWith((a) =>
        {
            this.ServerInfoList = a.Result;
            this.ServerInfoList.Add(new BasicInfo() { Name = "My First Server" });
            this.ServerInfoList.Add(new BasicInfo() { Name = "My Second Server" });
            this.ServerInfoList.Add(new BasicInfo() { Name = "My Third Server" });
        });
        task.Start();
    }
}