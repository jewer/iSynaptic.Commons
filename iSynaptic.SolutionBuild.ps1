task default -depends FullBuild

$FullBuildDependsOn = "Package", "CoreFinalize"

task FullBuild -depends $FullBuildDependsOn

task Package {
    "Packaging"
}

task CoreFinalize {
    "Finalizing..."
}
