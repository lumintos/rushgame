/****************************************
	RagdollController.js
	Disable and enable rigidbodies and colliders on the ragdoll
	
	Copyright 2013 Unluck Software	
 	www.chemicalbliss.com											
*****************************************/
#pragma strict
#pragma downcast 
private var boneRig : Component[];		// Contains the ragdoll bones
private var mass:float = .1;	// Mass of each bone
var projector:Transform;		// Shadow projector
var root:Transform;				// Assign the root bone to position the shadow projector
var _bloodColor:Color;
var _model:GameObject;
var _bodyMesh:Mesh;

var _explodeHeadPS:ParticleSystem;
var _head:GameObject;
var _headBone:Transform;

var _disableWhenDecapitated:GameObject[];
var _bodyPS:ParticleSystem;

private var _decapitated:boolean;

//Blinking
private var colorOriginal:Color;
private var color:Color;
private var _R:float = 2500;
private var _G:float = 2500;
private var _B:float = 2500;

private var _randomColor:boolean;
private var _blinkCounter:int;
private var _stopBlink:int;

function LateUpdate () {
	if(!collider.enabled && projector && root){
		projector.transform.position.x = root.position.x;
		projector.transform.position.z = root.position.z;
	}
}

function Start () {
	if(!root)
 	root = transform.FindChild("Root");
 	if(!projector)
 	projector = transform.FindChild("Blob Shadow Projector");
 	if(!_model)
 	_model = transform.FindChild("MicroMale").gameObject;
 	if(!_headBone)
 	_headBone = transform.FindChild("Head");
	boneRig = gameObject.GetComponentsInChildren (Rigidbody); 
	disableRagdoll();
	//Blinking
	colorOriginal = _model.renderer.material.color;
}

function Blink(times:int, speed:float, red:float, green:float , blue:float){
	CancelInvoke();
	_randomColor= false;
	_R = red;
	_G = green;
	_B = blue;
	_stopBlink = times;
	InvokeRepeating("BlinkInvoke", speed, speed);
}

function Blink(times:int, speed:float){
	CancelInvoke();
	_randomColor = true;
	_stopBlink = times;
	InvokeRepeating("BlinkInvoke", speed, speed);
}

function BlinkInvoke () {
	if(_blinkCounter < _stopBlink){
		if(_randomColor){
			color = new Color(Random.Range(1, 5) ,Random.Range(1, 5),Random.Range(1, 5),1);
		}else{
			color = new Color(_R , _G , _B ,1);
		}
		
		if(_model.renderer.material.color == colorOriginal){
			_model.renderer.material.color = color;
		}else{
			_model.renderer.material.color = colorOriginal;
		}
		_blinkCounter++;
	}else{
		_model.renderer.material.color = colorOriginal;
		_blinkCounter = 0;
		CancelInvoke();
	}
}

function disableRagdoll () {
	for (var ragdoll : Rigidbody in boneRig) {
		if(ragdoll.collider)
		ragdoll.collider.enabled = false;
		ragdoll.isKinematic = true;
		ragdoll.mass = 0.01;
	}
	collider.enabled = true;
	rigidbody.isKinematic = true;
	rigidbody.useGravity = false;
}
 
function enableRagdoll (delay:float, force:Vector3) {
	yield(WaitForSeconds(delay));
	for (var ragdoll : Rigidbody in boneRig) {
		if(ragdoll.collider)
		ragdoll.collider.enabled = true;
		ragdoll.isKinematic = false; 
		ragdoll.mass = mass;
		if(force.magnitude > 0)
		ragdoll.AddForce(force*Random.value);
	}
	GetComponent(Animator).enabled=false;
	collider.enabled = false;
	rigidbody.isKinematic = true;
	rigidbody.useGravity = true;
	for(var i:int; i < this._disableWhenDecapitated.length; i++){
				_disableWhenDecapitated[i].SetActive(false);
			}
}

function Decapitate (explode:boolean, delay:float, force:Vector3) {
	if(!_decapitated){
		_decapitated = true;
			_model.GetComponent(SkinnedMeshRenderer).sharedMesh = this._bodyMesh;
		if(_head){
			if(!explode){
				var h:GameObject = Instantiate(_head, _headBone.position, transform.rotation);
				h.transform.localScale = _headBone.localScale*transform.localScale.x;
				Physics.IgnoreCollision(gameObject.collider, h.collider);
				Destroy(_headBone.collider);
				h.renderer.sharedMaterial = _model.GetComponent(SkinnedMeshRenderer).sharedMaterial;
				if(force.magnitude > 0)
				h.rigidbody.AddForce(force*Random.value);
				h.rigidbody.AddTorque(Vector3(Random.Range(-10, 10),Random.Range(-10, 10),Random.Range(-10, 10)));
				h.transform.FindChild("Head PS").particleSystem.startColor = this._bloodColor;
				EnableCollisions(gameObject.collider, h.collider);
			}else{
				var e:GameObject = Instantiate(_explodeHeadPS.gameObject, _headBone.position, transform.rotation);
				e.particleSystem.startColor = this._bloodColor;
				Destroy(e, 2);
			}
			if(_bodyPS){
			_bodyPS.startColor = this._bloodColor;
			_bodyPS.Play();
			}
					
		}
		enableRagdoll(delay, force);
	}
}

function EnableCollisions(c1:Collider, c2:Collider){
	yield(WaitForSeconds(1));
	if(c2 && c1.enabled)
		Physics.IgnoreCollision(c1,c2, false);
}