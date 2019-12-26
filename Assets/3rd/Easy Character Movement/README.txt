EASY CHARACTER MOVEMENT
=======================

CHANGE LOG

Version 1.6

	*** MAJOR UPDATE, BREAKING CHANGES. PLEASE BACKUP BEFORE UPDATE.

	- Added support for steps and completely improved ground detection / character movement components. Eg: The character can now walk (if desired) on any surface up to 89 degrees.

	- Replaced the ground detection component. The new implementation (BaseGroundDetection / GroundDetection) is a much more robust and feature-rich component, capable of detect, report and query multiple grounding cases.	

	- The abstract class GroundDetection has been renamed to BaseGroundDetection.

	- The previous SphereGroundDetection, BoxGroundDetection and RaycastGroundDetection has been replaced with a new feature-rich GroundDetection component and marked as obsolete. This obsolete components will be removed in a following update.

	- The new GroundDetection component, automatically configures from your character's capsule collider.	

	- Improved ledge handling, the new GroundDetection allow to configure a desired ledge offset, this set how close / far a character can stand on a ledge without fall down.

	- Implemented flat-base feature. When on a ledge, the CharacterMovement component will treat the bottom of your capsule as flat instead of rounded. This avoids the situation where characters slowly lower off the side of a ledge (as their capsule 'balances' on the edge).

	- Implemented ground-snap feature. This help to maintain the character on ground no matter how fast it is running and not launch of ramps.

	- As part of the new ground-snap feature, the platform support is much more stable on moving platforms, the character will maintain the platform no matter how fast it moves, or its friction settings.

	- CharacterMovement now correctly apply platform angular velocity when on rotating platforms. Exposed platformAngular velocity property.

	- Added new methods (ComputeGroundHit) to allow query 'ground' info at any time.

	- Implemented continuous collision detection, this greatly improve the character landing, no matter how fast the character is falling, it will always land safely.

	- Refactored all BaseCharacterController private fields to protected. This allow derived classes access them an implement custom jump logic, etc.

	- Added a new method UpdateRotation to BaseCharacterController, this helps to easily modify the default ecm rotation method (rotate towards movement direction).

	- New gizmos, GroundDetection now show if character is on a step, on a ledge 'solid' side, on a ledge 'empty' side, on valid ground, on invalid ground, and not on ground. Please reffer to documentation for a fully description.

	- Removed OnCollisionXXX events dependency.

	- Minor Bug Fixes.

	- Code refactored and cleaned comments and tooltips fixes.
	

Version 1.5

    - Unity 2017 support

    - MouseLook class can be extended, just like other Base controllers.

    - Minor bugs fixes.


Version 1.4

	*** MAJOR UPDATE, BREAKING CHANGES. PLEASE BACKUP BEFORE UPDATE.

	- Major update, the whole package has been refactored and restructured to be much more organized and easier to work with.

	- NEW physical model, greatly improved movement physics adding friction, this helps to achive a much more responsive control
	  even at low acceleration values.

	- Improved BaseCharacterController, this has been refactored making it much more robust and easier to extend.

	- Introduced 2 new base controllers, 'BaseAgentController' and 'BaseFirstPersonController'.

	- 'BaseAgentController' new base controller for NavMeshAgent based characters.

	- 'BaseFirstPersonController' new base controller for First Person character controller.

	- New examples to show how to work with the new included controllers.

	- Improved and more extensive documentation.

	- Removed the use / requirement of Resources folder.

	- Revised / optimized code, and improved in-code comments and tooltips.


Version 1.3

	- Improved jump physics. In the new method, we apply a proportional extra jump power (acceleration) to perform variable height jump,
	  this offers a better jump control and removes any floaty jetpack feel.

 	- Added a new maxRiseSpeed property to CharacterMovement component. This helps to limit the maximum rising velocity along y+ axis.

 	- Added tooltips to main components, this helps to tweak its values without the need to look in documents / code.

 	- ECM now belongs to the Scripting/Physics asset store category. This removes the "one license per seat" restriction imposed to editor extensions.


Version 1.2

	- Added a jump tolerance time property to base character controller.
	  This helps to manage how early before hitting the ground you can press jump, and still perform the jump.

	- Added a new Raycast ground detection component.

	- Added a CustomCharacterController example.
	  This shows how to create a custom character controller to perform the movement relative to camera.

	- Fixed a minor bug where some of the controllers were not applying braking drag.


Version 1.1

	- Fixed a minor bug related to demo scene lightmap size.

	- Added a simple demos scene, to faster start.

	- Exposed all ground info from movement component.
	
	- Improved character braking, braking is composed of friction (velocity-dependent drag) and constant deceleration.


VERSION 1.0

	- Initial release.



DISCLAIMER & LEGAL INFORMATION

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
PARTICULAR PURPOSE.

YOU MAY NOT REDISTRIBUTE THIS SOURCE CODE IN WHOLE OR IN PART
WITHOUT WRITTEN CONSENT FROM THE CONTENT AUTHOR OR COPYRIGHT HOLDER.