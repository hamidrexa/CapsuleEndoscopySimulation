#pragma strict

var initialVelocity : Vector3;
var initialAngularVelocity : Vector3;

function Start() {
    rigidbody.velocity = initialVelocity;
    rigidbody.angularVelocity = initialAngularVelocity;
}
