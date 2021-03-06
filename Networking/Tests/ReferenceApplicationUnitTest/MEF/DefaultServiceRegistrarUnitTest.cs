﻿//___________________________________________________________________________________
//
//  Copyright (C) 2019, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community at GITTER: https://gitter.im/mpostol/OPC-UA-OOI
//___________________________________________________________________________________

using CommonServiceLocator;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Diagnostics;
using System.Linq;
using UAOOI.Networking.Core;
using UAOOI.Networking.DataLogger;
using UAOOI.Networking.ReferenceApplication.Core;
using UAOOI.Networking.ReferenceApplication.MEF;
using UAOOI.Networking.SimulatorInteroperabilityTest;

namespace UAOOI.Networking.ReferenceApplication.UnitTest.MEF
{

  [TestClass]
  public class DefaultServiceRegistrarUnitTest
  {

    [TestMethod]
    public void RegisterRequiredServicesIfMissingNullArgumentTestM()
    {
      using (AggregateCatalog newCatalog = DefaultServiceRegistrar.RegisterServices(null)) { }
    }
    [TestMethod]
    public void RegisterRequiredServicesIfMissingTest()
    {
      using (AggregateCatalog newCatalog = DefaultServiceRegistrar.RegisterServices(null))
      {
        using (CompositionContainer _container = new CompositionContainer(newCatalog))
        {
          foreach (ComposablePartDefinition _part in _container.Catalog.Parts)
            foreach (var export in _part.ExportDefinitions)
              Debug.WriteLine(string.Format("Part contract name => '{0}'", export.ContractName));
          Assert.AreEqual<int>(10, _container.Catalog.Parts.Count());
          MainWindow _MainWindowExportedValue = _container.GetExportedValue<MainWindow>();
          Assert.IsNotNull(_MainWindowExportedValue);
          Assert.IsNotNull(_MainWindowExportedValue.MainWindowViewModel);
          IEnumerable<INetworkingEventSourceProvider> _diagnosticProviders = _container.GetExportedValues<INetworkingEventSourceProvider>();
          Assert.AreEqual<int>(2, _diagnosticProviders.Count<INetworkingEventSourceProvider>());
        }
      }
    }
    [TestMethod]
    public void RegisterRequiredServicesIfMissingAndUDPMessageHandler()
    {
      AggregateCatalog _catalog = new AggregateCatalog(new AssemblyCatalog("UAOOI.Networking.UDPMessageHandler.dll"), new AssemblyCatalog("UAOOI.Networking.SimulatorInteroperabilityTest.dll"));
      AggregateCatalog _newCatalog = DefaultServiceRegistrar.RegisterServices(_catalog);
      int _disposingCount = 0;
      using (CompositionContainer _container = new CompositionContainer(_newCatalog))
      {
        IServiceLocator _serviceLocator = new ServiceLocatorAdapter(_container);
        ServiceLocator.SetLocatorProvider(() => _serviceLocator);
        Assert.AreEqual<int>(13, _container.Catalog.Parts.Count<ComposablePartDefinition>());
        foreach (ComposablePartDefinition _part in _container.Catalog.Parts)
        {
          Debug.WriteLine("New Part");
          foreach (ImportDefinition _import in _part.ImportDefinitions)
            Debug.WriteLine(string.Format("Imported contracts name => '{0}'", _import.ContractName));
          foreach (ExportDefinition _export in _part.ExportDefinitions)
            Debug.WriteLine(string.Format("Exported contracts name => '{0}'", _export.ContractName));
        }
        //UDPMessageHandler
        IMessageHandlerFactory _messageHandlerFactory = _container.GetExportedValue<IMessageHandlerFactory>();
        Assert.IsNotNull(_messageHandlerFactory);
        INetworkingEventSourceProvider _baseEventSource = _messageHandlerFactory as INetworkingEventSourceProvider;
        Assert.IsNull(_baseEventSource);
        IEnumerable<INetworkingEventSourceProvider> _diagnosticProviders = _container.GetExportedValues<INetworkingEventSourceProvider>();
        Assert.AreEqual<int>(3, _diagnosticProviders.Count<INetworkingEventSourceProvider>());
        // DataLogger
        EventSourceBootstrapper _eventSourceBootstrapper = _container.GetExportedValue<EventSourceBootstrapper>();
        LoggerManagementSetup _logger = _container.GetExportedValue<LoggerManagementSetup>();
        _logger.DisposeCheck(x => _disposingCount++ );
        Assert.IsNotNull(_logger.BindingFactory);
        Assert.IsNotNull(_logger.ConfigurationFactory);
        Assert.IsNotNull(_logger.EncodingFactory);
        Assert.IsNotNull(_logger.MessageHandlerFactory);
        SimulatorDataManagementSetup _simulator = _container.GetExportedValue<IProducerDataManagementSetup>() as SimulatorDataManagementSetup;
        Assert.IsNotNull(_simulator);
        Assert.IsNotNull(_simulator.BindingFactory);
        Assert.IsNotNull(_simulator.ConfigurationFactory);
        Assert.IsNotNull(_simulator.EncodingFactory);
        Assert.IsNotNull(_simulator.MessageHandlerFactory);
        _simulator.DisposeCheck(x => _disposingCount++);
        Assert.AreEqual<int>(0, _disposingCount);
      }
      Assert.AreEqual<int>(2, _disposingCount);
    }

  }
}

