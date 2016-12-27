# MeasureIt

## Overview

Welcome to **MeasureIt**, a .NET library that enables performance measurement of your web and service oriented architectures. **MeasureIt** exposes ``Performance Counters`` organized into ``Performance Counter Categories``, which may be viewed by *Windows Performance Monitor*.

**MeasureIt** requires that you have access to the code you want to instrument. At minimum, you need to be able to derive ``.NET`` type information in the form of *subclasses* and *overridden virtual methods*. This is due to the fact that ``.NET`` ``Attributes`` are used in order to .

### Performance Counter Adapters

A half dozen or so counter adapters are available out of the box for your convenience. Contributions are welcome, or feel free to adapt counters of your own.

#### Begin/End Measurement Boundaries

Adaptation involves a measurement being bounded by ``BeginMeasurement(IPerformanceMeasurementDescriptor descriptor)`` and ``EndMeasurement(TimeSpan elapsed, IPerformanceMeasurementDescriptor descriptor)``. ``BeginMeasurement`` is called prior to the method invocation, and ``EndMeasurement`` is called afterward, along with a provided ``elapsed``.

### Performance Counter Category Adapters

Out of the box, a ``DefaultPerformanceCounterCategoryAdapter`` is provided for convenience; but for most uses, you should derive from ``PerformanceCounterCategoryAdapterBase``.  This exposes key elements of your ``Performance Counter Category`` (or ``Categories``) necessary to support the measurement framework.

### Service Instrumentation

You have a choice of <em>[Dependency Injection](http://en.wikipedia.org/wiki/Dependency_injection)</em> containers when deciding to instrument your services: ``MeasureIt.Autofac``, or ``MeasureIt.Castle.Windsor``, via NuGet packages.

In either case, service instrumentation depends upon ``Castle.DynamicProxy.IInterceptor`` to facilitate instrumentation, via an established ``MeasureIt.Castle.Interception.IMeasurementInterceptor``.

You must also register ``MeasureIt.Castle.Interception.IInterceptionMeasurementProvider`` and ``MeasureIt.Castle.Interception.InterceptionMeasurementProvider`` with your container.

#### Autofac Service Instrumentation

The following extension methods are provided to facilitate interception enablement and measurement. The same sorts of patterns apply when instrumenting for Web API, and, eventually, into MVC as well, so we will spend a little more time discussing the patterns in depth for service oriented instrumentation.

Method Name|Generic Parameters|Method Parameters|Description|Assumptions
---|---|---|---|---
``Autofac.ContainerBuilder.EnableMeasurements``|``TInterface : class, IRuntimeInstrumentationDiscoveryService, TService : class, TInterface``|``Action<IInstrumentationDiscoveryOptions> optsCreated = null``|Enables measurements via the specified Discovery Service, defaults to ``MeasureIt.Castle.Interceptor.MeasurementInterceptor``|
``Autofac.ContainerBuilder.EnableMeasurements``|``TInterface : class, IRuntimeInstrumentationDiscoveryService; TService : class, TInterface; TInterceptor : class, IMeasurementInterceptor``|``Action<IInstrumentationDiscoveryOptions> optsCreated = null``|Enables measurements via the specified Discovery Service|
``Autofac.ContainerBuilder.EnableMeasurementInterception``|``TImplementer : class; TInterceptor : class, IMeasurementInterceptor``|``Action<AutofacProxyGenerationOptions> optsProxyGeneration = null``|Enables measurements via the specified Discovery Service|Options ``EnableInterception`` must be set to either ``AutofacEnableInterceptionOption.Class`` (default) or ``AutofacEnableInterceptionOption.Interface``
``Autofac.IContainer.MeasureInstance``|``T : class; TInterceptor : class, IMeasurementInterceptor``|``Action<AutofacProxyGenerationOptions> optsProxyGeneration = null``|Enables measurements via the specified Discovery Service|At least one ``Castle.DynamicProxy.IInterceptor`` must be registered, namely the ``MeasureIt.Castle.Interception.MeasurementInterceptor``, or a derivation thereof

Examples:

```C#
// Assumes InitializeOptions is defined elsewhere.
// Which defaults to MeasurementInterceptor.
builder.EnableMeasurements<IInstallerInstrumentationDiscoveryService, InstallerInstrumentationDiscoveryService>(InitializeOptions);

builder.EnableMeasurements<IInstallerInstrumentationDiscoveryService, InstallerInstrumentationDiscoveryService, MeasurementInterceptor>(InitializeOptions);
```

Most times you will want the ``IRuntimeInstrumentationDiscoveryService``, but sometimes you may also want the ``IInstallerInstrumentationDiscoveryService``, especially if you want to install the category for any reason. Installation can be because of a contextualized installer, or because runtime services are warming up upon container resolution.

When your container resolves the service, or you request to enable measurement on an already instantiated object, whether a service or not, you should end up with a fully measured service or object. This does not include installation of your counter categories, per se. For that you need to resolve the ``IInstallerInstrumentationDiscoveryService`` and go about installing.

Once you have identified a category adapter, one or more desired counter adapter(s), and have your container configured, then you can go about instrumenting your services or measurable objects. You do this by decorating the method (or methods) with the ``MeasureIt.MeasurePerformanceAttribute``. ``MeasurePerformanceAttribute`` is where all of these concerns are welded together, so to speak. You may name the measurement or not; ``MeasureIt`` will make an effort to identify the measurements by the member signature in full namespace.

Non-public methods may be measured depending on how you have configured the ``MeasureIt.Discovery.InstrumentationDiscoveryOptions.MethodBindingAttr``. Additionally, the discovery agents go to great lengths to ensure that only the most recent decoration for a given member signature is used to facilitate performance measurement.

Example:

```C#
[MeasurePerformance("Counter Name"
    , typeof(DefaultPerformanceCounterCategoryAdapter)
    , typeof(AverageTimePerformanceCounterAdapter)
    , PublishEvent = true // default = true
    , PublishCounters = true // default = true
    , InstanceLifetime = PerformanceCounterInstanceLifetime.Process // default = PerformanceCounterInstanceLifetime.Process
    , ThrowPublishErrors = true // default = false
    , SampleRate = 1d // default 1.0d
    )]
public void InstrumentedMember()
{
  // Does something worth measuring...
}
```

The ``CategoryType``, ``AdapterType``, and ``OtherAdapterTypes`` are all fairly self explanatory. Other measurement options are as follows:

Option|Type|Default|Description
---|---|---|---
``PublishEvent``|``System.Boolean``|``true``|Indicates whether to publish an event.
``PublishCounters``|``System.Boolean``|``true``|Indicates whether to publish counters.
``MayProceedUnabated``|``System.Boolean``|``!(PublishEvent || PublishCounters)``|Derived from ``PublishEvent`` and ``PublishCounters`` whether to proceed with the measurement.
``InstanceLifetime``|``System.Diagnostics.PerformanceCounterInstanceLifetime``|``PerformanceCounterInstanceLifetime.Process``|Defines the counter instance lifetime for the categorized counters; most times this will be ``PerformanceCounterInstanceLifetime.Process``, but sometimes you may want to configure ``PerformanceCounterInstanceLifetime.Global``, so we have exposed this feature.
``ThrowPublishErrors``|``System.Boolean``|``false``|Indicates whether to throw ``System.Exception`` encountered at any time during the target invocation or measurement context.
``SampleRate``|``System.Double``|``1.0``|Provides a means to throttle the diagnostic measurement rate.

#### Castle Windsor Service Instrumentation

The patterns for *Castle Windsor* are fairly similar to *Autofac*, so we will not repeat ourselves here. There are some subtle differences, such as whether ``Autofac.ContainerBuilder`` or ``Castle.Windsor.IWindsorContainer`` is used, but other than that, the concepts are very similar, if not the same.

Method Name|Generic Parameters|Method Parameters|Description|Assumptions
---|---|---|---|---
``Castle.MicroKernel.ComponentRegistration<T>.MeasureUsing``|``T : class; TInterceptor : class, IMeasurementInterceptor``|*(none)*|Enables measurements via the specified Discovery Service|At least one ``Castle.DynamicProxy.IInterceptor`` must be registered, namely the ``MeasureIt.Castle.Interception.MeasurementInterceptor``, or a derivation thereof

``MeasureUsing`` is analogous to the ``Autofac`` ``MeasureInstance`` with a few minor differences.

Otherwise the patterns are very much the same, notwithstanding choice of container.

### Web API Instrumentation

The concepts with Web API instrumentation are very similar to Interception where *Counter Adapters*, *Category Adapters*, *Discovery Agents*, and *Discovery Services* are concerned. The only differences are in the enablement of measurement services and decoration of API actions.

As with *Interception*, initially I am supporting *Autofac* and *Castle Windsor* out of the box. I will enumerate the *Autofac* features, with the *Castle Windsor* features being very similar in nature.

#### Autofac Measurement Service Enablement

Method Name|Generic Parameters|Method Parameters|Description|Assumptions
---|---|---|---|---
``Autofac.ContainerBuilder.EnableApiMeasurements``|``TInterface : class, IHttpActionInstrumentationDiscoveryService; TService : class, TInterface; TProvider : class, ITwoStageMeasurementProvider``|``Action<IInstrumentationDiscoveryOptions> optsCreated = null``|Enables API measurements via the specified Discovery Service, defaults to ``MeasureIt.Castle.Interceptor.MeasurementInterceptor``|
``Autofac.ContainerBuilder.RegisterApiService``|``TInterface : class; TService : class, TInterface``|*(none)*|Registers a service for API
``System.Web.Http.HttpConfiguration.ReplaceService``|``TInterface : class; TService: class, TInterface``|``IContainer container``|Replaces the service in the ``System.Web.Http.HttpConfiguration.Services`` collection|That the service interface has been registered
``Autofac.ContainerBuilder.RegisterApiServices``|*(none)*|*(none)*|Registers a common set of services for API|At present I am replacing the ``AutofacHttpControllerActivator`` implementation for ``IHttpControllerActivator``, but this may change in the future

Examples:

```C#
builder.EnableApiMeasurements<
    IHttpActionInstrumentationDiscoveryService,
    HttpActionInstrumentationDiscoveryService,
    HttpActionMeasurementProvider>(o =>
    {
        o.Assemblies = new[]
        {
            // Assuming I have a MyApiController in my solution.
            typeof(MyController).Assembly,
            typeof(AverageTimePerformanceCounterAdapter).Assembly
        };
    });
```

Usage of these services should be able to coexist seamlessly with the ``Interception`` measurements.

#### API Action Decoration

### NuGet Distrubition

There are half a dozen or so *MeasureIt* packages that build upon the core or more basic either *MeasureIt* or third-party packages. You will typically operate through the ``MeasureIt.Autofac`` or ``MeasureIt.Castle.Windsor`` packages for service oriented measurement, and ``MeasureIt.Autofac.AspNet.WebApi`` or ``MeasureIt.Castle.Windsor.AspNet.WebApi`` packages for Web API measurement. All requisite dependencies should be resolved following that.

### Future Goals

* There is enough detail here that I may consider decomposing this into more of a Wiki presentation than a README.

* Presently, Web API is supported. Eventually, I want to introduce MVC support in the web features.

* I am anticipating API/MVC core to be supported by the ASP.NET team in the not too distant future, if it is not already. I will look to add that support in lieu of legacy assemblies, frameworks, etc.
