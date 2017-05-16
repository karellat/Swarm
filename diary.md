cd # Diary of project
# FrameWork for: Evolutionary Algorithms for the Control of Heterogeneous Robotic Swarm

## TODO: 
REFACTOR WHOLE PROJECT
Add consuming fuel to effectors
## Q&A: 
### Implementation:  
* How long should be GetString();
* How to log? 
### Evolution: 
* Radio sensor, what should it return?
* Added memory, does it make sense? 
### Days of actual sollution

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



	