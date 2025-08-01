#!/bin/bash

echo "Install tools if not present"
dotnet tool install --global coverlet.console
dotnet tool install --global dotnet-reportgenerator-globaltool

echo "Clean and build solution"
dotnet restore Ambev.DeveloperEvaluation.sln
dotnet build Ambev.DeveloperEvaluation.sln --configuration Release --no-restore

echo "Run tests with coverage"
dotnet test tests/Ambev.DeveloperEvaluation.Unit/Ambev.DeveloperEvaluation.Unit.csproj --no-restore --verbosity normal /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura /p:CoverletOutput=./TestResults/coverage.cobertura.xml /p:Exclude="[Ambev.DeveloperEvaluation.ORM]*.Migrations.*"

echo "Generate coverage report"
reportgenerator -reports:"./tests/**/TestResults/coverage.cobertura.xml" -targetdir:"./TestResults/CoverageReport" -reporttypes:Html

echo "Removing temporary files"
rm -rf bin obj

echo ""
echo "Coverage report generated at TestResults/CoverageReport/index.html"