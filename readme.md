# What are ObjectBlocks?

It started as an idea to move ripple off of XML and turned into something that might be handy in configuration scenarios. FubuObjectBlocks
is just a VERY simple object notation used to express simple objects. The values are parsed and pushed through the model binding component in FubuCore
so you get all of the conventional goodness and power we know and love.

While useful for tools like ripple, FubuObjectBlocks might prove to be useful in the AppSettingsProvider scenarios as well.

### Example syntax:

<code>
solution:
  name 'ripple'
  nuspecs 'packaging/nuget'
  srcFolder 'src'
  buildCmd 'rake'
  fastBuildCommand 'rake compile'
  constraints:
    float 'current'
    fixed 'current,nextMajor'
 
feed 'http://build.fubu-project.org/guestAuth/app/nuget/v1/FeedService.svc', mode: 'float', stability: 'released'
feed 'http://nuget.org/api/v2', mode: 'fixed', stability: 'released'
 
nuget 'FubuCore', version: '~>1.1.0'
nuget 'NuGet.Core', version: '2.5.0', mode: 'fixed'
nuget 'NUnit', version: '2.5.10.11092', mode: 'fixed'
nuget 'RhinoMocks', version: '3.6.1', mode: 'fixed'
nuget 'structuremap', version: '2.6.3', mode: 'fixed'
nuget 'structuremap.automocking', version: '~>1.1.0', mode: 'fixed'
</code>

A few things to note:

* Nested objects are denoted with ":"
* Collections can either be defined as a nested object or inline as shown in 'feed' and 'nuget'. 
* Inline objects must define property names ending with ":". The first value is known as the implicit value. See the unit tests for examples.