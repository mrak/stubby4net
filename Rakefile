require 'albacore'
require 'version_bumper'

description = "A small server for stubbing external systems during development."
title = "stubby4net"
product = "stubby"
company = "Eric Mrak"
copyright = "Copyright 2013"
nuspec_file = 'stubby\stubby.nuspec'

desc "Generate nuspec file"
nuspec :nuspec do |ns|
   ns.id = product
   ns.version = bumper_version.to_s.strip
   ns.authors = company
   ns.description = description
   ns.title = title
   ns.language = "en_US"
   ns.licenseUrl = "https://raw.github.com/mrak/stubby4net/master/LICENSE.txt"
   ns.projectUrl = "https://github.com/mrak/stubby4net"
   ns.iconUrl = "http://stub.by/favicon.ico"
   ns.tags = "stub mock testing server"
   ns.file 'bin\stubby.exe', "lib"
   ns.copyright = copyright

   ns.output_file = nuspec_file
end

desc "Create nupkg files"
exec :package => :nuspec do |cmd|
   cmd.command = 'nuget.exe'
   cmd.parameters 'pack ' + nuspec_file + ' -Symbols -BasePath stubby -OutputDirectory nuget'
end

desc "Generate AsseblyInfo.cs"
assemblyinfo :assemblyinfo do |asm|
   asm.version = bumper_version.to_s.strip
   asm.file_version = bumper_version.to_s.strip
   asm.company_name = company
   asm.product_name = product
   asm.title = title
   asm.description = description
   asm.copyright = copyright

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

desc "Merge together assemblies"
exec :merge => :build do |ilm|
   ilm.command = 'ilmerge'
   ilm.parameters '/wildcards /out:stubby\bin\stubby.exe stubby\bin\Release\stubby.exe stubby\bin\Release\*.dll'
   puts 'merging...'
end

desc "Test"
nunit :test => :build do |nunit|
   nunit.command = 'nunit-console'
   nunit.assemblies 'unit/bin/Release/unit.dll', 'integration/bin/Release/integration.dll'
end

task :default => [:test, :merge]
