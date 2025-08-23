# Ruta de tu DbContext y del proyecto
$context = "ApplicationDbContext"
$outputDir = "Controllers"
$project = "worldcities.server.csproj"

# Lista de modelos a generar
$models = @(
    "City",
    "Country",
    "Region"
    # agrega aquí todas las entidades que quieras
)

foreach ($model in $models) {
    Write-Host "Generando controlador para $model..."
    dotnet aspnet-codegenerator controller `
        --controllerName "${model}sController" `
        --model $model `
        --dataContext $context `
        --relativeFolderPath $outputDir `
        --useAsyncActions `
        --project $project
}
