# Diary of project
# FrameWork for: Evolutionary Algorithms for the Control of Heterogeneous Robotic Swarm

## TODO: 

## Q&A: 
### Implementation:  
* How long should be GetString();
### Evolution: 
* 


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