require 'albacore'
require 'version_bumper'

desc "AssemblyInfo"
assemblyinfo :assemblyinfo do |asm|
   asm.version = bumper_version.to_s.strip
   asm.file_version = bumper_version.to_s.strip
   asm.company_name = "Eric Mrak"
   asm.product_name = "stubby"
   asm.title = "stubby4net"
   asm.description = "A small server for stubbing external systems during development."
   asm.copyright = "Copyright 2013"

   asm.com_visible = false
   asm.com_guid = "afa901aa-9750-470b-9d6f-a03ed5b5ae92"

   asm.namespaces "System.Runtime.CompilerServices"
   asm.custom_attributes :InternalsVisibleTo => "unit"

   asm.output_file = "stubby/Properties/AssemblyInfo.cs"
end

desc "Build"
msbuild :build => :assemblyinfo do |msb|
   msb.properties = { :configuration => :Release }
   msb.targets = [ :Clean, :Build ]
   msb.solution = "stubby4net.sln"
end

desc "ILMerge"
exec :merge => :build do |ilm|
   ilm.command = 'C:\Chocolatey\bin\ILMerge.bat'
   ilm.parameters '/wildcards /out:stubby\bin\stubby.exe stubby\bin\Release\stubby.exe stubby\bin\Release\*.dll'
   puts 'merging...'
end

desc "Test"
nunit :test => :build do |nunit|
   nunit.command = 'C:/Program Files (x86)/Mono-3.0.6/lib/mono/4.5/nunit-console.exe'
   nunit.assemblies 'unit/bin/Release/unit.dll', 'integration/bin/Release/integration.dll'
end

task :default => [:test, :merge]
