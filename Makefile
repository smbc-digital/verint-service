coverage:
	cd ./tests && dotnet test //p:CollectCoverage=true /p:Exclude=\"[*],[xunit*]*,[*]verint_service-tests*\" //p:ExcludeByAttribute="ExcludeFromCodeCoverage" //p:CoverletOutputFormat=lcov