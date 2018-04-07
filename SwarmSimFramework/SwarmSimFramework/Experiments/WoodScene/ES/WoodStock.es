WoodScene
#ES SETTINGS
RobotModels:WoodRobots.WoodWorkerRobotMem-4
BrainModels:8=WoodRobots.WoodWorkerRobotMem()-WoodWWalkES\[i]thread\best0_WoodWorkerM
f:WoodSceneFitnessCounter
#ES
NumberOfGenerations:100
sigma:0,1
alpha:0,05
SingleStepPopulationSize:20
NumberOfMapIterations:2000
Name:WoodCollectingES
WorkingDir:WoodCollectES
Elitism:false
#F SETTINGS
ValueOfCutWood:100
ValueOfCollision:-1
ValueOfDiscoveredTree:0
ValueOfStockedWood:1000
ValueOfContaineredWood:100
ValueOfContaineredNoWood:-100
#MAP SETTINGS 
AmountOfTrees:200
AmountOfWoods:200

