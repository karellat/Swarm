
#ES SETTINGS
MapModel:WoodScene
RobotModels:WoodRobots.WoodWorkerRobot()-4;WoodRobots.ScoutCutterRobot()-5
BrainModels:WoodRobots.ScoutCutterRobotMem()-G;WoodRobots.WoodWorkerRobot()-G
BrainModels:WoodRobots.ScoutCutterRobotMem()-path[i];WoodRobots.WoodWorkerRobot()-path[i]
NumberOfGenerations:100
sigma:0.1f
alpha:0.05f
SingleStepPopulationSize:20
MapIteration:2000
Name:WoodWalkingES
WorkingDir:WoodWalkES
f:fitnessCounter
#F SETTINGS
ValueOfCutWood:1
ValueOfCollision:0
ValueOfDiscoveredTree:100
ValueOfStockedWood:1000
ValueOfContaineredWood:0
#MAP SETTINGS 
AmountOfTrees:50
AmountOfWood:200

