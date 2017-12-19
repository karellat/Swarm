WoodScene
#ES SETTINGS
RobotModels:WoodRobots.WoodWorkerRobotMem-4;WoodRobots.ScoutCutterRobotMem-5
BrainModels:8=WoodRobots.WoodWorkerRobotMem()-WoodCollectES\[i]thread\best0_WoodWorkerM;WoodRobots.ScoutCutterRobotMem()-WoodCutES\[i]thread\best0_WoodCutterM
f:WoodSceneFitnessCounter
#ES
NumberOfGenerations:100
sigma:0,1
alpha:0,05
SingleStepPopulationSize:20
NumberOfMapIterations:2000
Name:WoodCoopES
WorkingDir:WoodCoopES
Elitism:false
#F SETTINGS
ValueOfCutWood:1
ValueOfCollision:0
ValueOfDiscoveredTree:0
ValueOfStockedWood:100
ValueOfContaineredWood:10
#MAP SETTINGS 
AmountOfTrees:400
AmountOfWoods:0

