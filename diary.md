# Diary of project
# FrameWork for: Evolutionary Algorithms for the Control of Heterogeneous Robotic Swarm

## TODO: 
REFACTOR WHOLE PROJECT
Add consuming fuel to effectors
- [x] Omezit signály na pevný počet(3) tzn. upravit sensor na čtení tří signálů
- [x] Barvičky udělat vracení jednotlivých proměnných
- [x]  Memory - Effector, Sensor
- [x] Hyperbolický tangens
- [x] ReLU,PReLU
- [x] lineární lomená fce
- [x] váhy float - (-0.1,0.1) náhodně
- [x] mutace, gausovské mutace z normálního rozdělení, málo..., max. 10 vah 
- [x] %1,%0.5 z vah a mutace časté, 
- [ ] Evoluční stragii, Diferenciální evoluce, f=0.8 
- [ ] Testování sensorů v GUI 
- [ ] Kreslení grafů
- [ ] Jednoduchý experiment na chození
- [x] LineTypeSensor
- na o málo tisicina 


- TEXT: 
vyplnit sablonu
Uvod - 2-3 stranky, 
Povidani obecne - NEAT, Neuronky, Evoluce
Related work - bakalarky,Fillipi, Papery 4-5, jak to delal nekdo jinej
Scenare, experimenty - fungovani frameworky, fungovani experimentu, mozky robotu 
Zaver - uvod v minulym case, shrnuti, jak zlepsit

## Q&A: 
### Implementation:  
* How long should be GetString();
* How to log? 
### Evolution: 
*
## Scenario
### Mineral refactoring
* Scout Robot
	* container size: 0
	
	* 3x FuelLineSensor
	* 3x LineTypeeSensor
	* LocatorSensor
	* TypeCircleSensor
	* 3x TouchSensor
	* RadioSensor
    
	* TwoWheelMotor
	* RadioTransmitter

* Worker Robot
	* container size: 5


	* 3x FuelLineSensor
	* 3x LineTypeSensor
	* 3x TouchSensor 
	* LocatorSensor
	* RadioSensor
	
	* RadioTransmiter
	* TwoWheelMotor
	* Picker

* Refactor Robot
	* container size: 10 

	* 1 LineTypeSensor
	* 1 FuelLineSensor 
	* 3 TouchSensor
	* RadioSensor
	
	* 4x Picker
	* MineralRefactor 
	* RadioSensor
	* RadioTransmitter

### Wood Cutting 
* Scout Cutter 
	* container size: 0
	* 3x FuelLineSensor
	* 3x LineTypeeSensor
	* LocatorSensor
	* TypeCircleSensor
	* 3x TouchSensor
	* TwoWheelMotor
	* RadioTransmitter
	* RadioSensor
	* WoodRefactor 

* Worker Robot
	* container size: 5
	* TwoWheelMotor
	* Picker
	* 3x FuelLineSensor
	* 3x LineTypeSensor
	* 3x TouchSensor 
	* LocatorSensor
	* RadioSensor
	* RadioTransmiter

### Competetive
* Light scout unit  
	* container size: 0
	* 3xFuelLineSensor
	* 3xLineTypeeSensor
	* LocatorSensor
	* 3x TouchSensor
	* TwoWheelMotor
	* RadioTransmitter
	* RadioSensor
	* 1x Weapon

* Heavy robot unit
	* container size: 5
	* TwoWheelMotor
	* 3xFuelLineSensor
	* 3xLineTypeSensor
	* 3x TouchSensor 
	* LocatorSensor
	* RadioSensor
	* RadioTransmiter
	* 3 Weapon high damage


## Days of actual sollution

* 05.05.2017 - 
	* refactoring old solutions
	* object design(IEntity was not created due to many abstract classes, no way to avoid virtual call)
	* sensor -> child of entity & inherit ISensor interface
	* use more asserts
	* brain serializer/deserializer 
* 06.05.2017 - 
	* refactoring Map => Constructor, Reset, MakeStep 
	* partial project Intersection2D 
* 07.05.2017 
	* Intersection2D finnished, tested 
	* map implementation finnished 
	* add new method MoveTo to Entity
	* Circle Entity & Line Entity refactored prepared for testing
* 08.05.2017 
	* LineEntity tests created
	* LineEntity squashed bugs 
	* CircleEntity tests created
	* CircleEntity bug squashed
	* added new method to Circle move entity for  given length
* 09.05.2017 
	* Line & Circle collision in the map enviroment tested, fixed (wrong ordering of border points)
	* Intersection2D CircleLinesegmentCollision() - Linesegment border check fixed 
	* ISensor & IEffector - added new methods Clone, Connect(set same normalize values as robot)
	* Robot - implementation refactored, not tested
	* Fuel - entity added 
* 10.05.2017 
	* Line sensor added  
	* Touch sesnsor added 
* 11.05.2017
	* Circle Entity MoveTo method = direction of move is set from RotationMiddle
	* Circle Entity RotataRadians method = take GetRotation Middle when rotate
	* Collision with types addded to map 
	* TypeLineSensor added
	* Entity rotationMiddle added to the Entity
* 12.05.2017 
	* Touch Sensor tested
	* bounds struct created 
	* NormalizeFunc struct created
	* TypeLineSensor implemented
* 13.05.2017
	* LineTypeSensor tested
	* Map RadioCollision refactor 
	* RadioEntity implemented 
	* static variable(RadioEntity) set bounds of used signal values (-100,100)
	* RadioSensor implemented 
	* RadioSensor tested
	* FuelLineSensor implemented 
	* FuelLineSensor tested
	* LocatorSensor implemented
* 15.05.2017
	* LocatorSensor tested 
	* ColorIntersection definition
	* ColorCollision added to Map
	* TypeCircleSensor implemented & tested
	* TwoWheelMotor implemented & tested, change dir. of rotation
* 16.05.2017
	* RadioTransmitter implemented & tested 
	* Add container stack to RobotEntity
	* Picker implemented & tested (3 modes, Pick Up,Put,Idle), pick up objects of max picker length
	* Mineral Refactor implemented
	* Mineral Entity (with capacity of created Fuel, and cycles to refactor)
	* Mineral Refactor tested
	* mortality and health of RobotEntity
	* map have to check Alive of robot
	* WeaponEffector implemented & tested

* 17.05.2017
	* IRobotBrain interface declared
	* WeightMeanBrain added
	* WeightMeanBrain tested

* 18.05.2017
	* consulation
	* StateEffector remade interface
	* added WoodRefactor
	* added WoodEntity
* 20.05.2017
	* activation function added (Tanh, PReLu)
	* Percepton simple implementationa added (Generating new brains, Mutate this brains)
	* Math.Net Numerics library added to project due to Normal Distributed random values
* 21.05.2017
	* GUI refactoring
	* IExperiment interface prepared 
* 23.05.2017
	* Testing sensors & effectors 
	* GUI & logging	
* 25.05.2017
	* Sensor, Effector logging
	* Memory effector/sensor added
	* Add metainfos to Visualisation
	* fix closing method 

* 27.05.2017 
	* Robot decition function fixed(Decition base on read values !)
	* RobotBrain serialization 

* 28.05.2017
	* Walking experiment definition 
	
	



	