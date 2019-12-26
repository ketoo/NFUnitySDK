using ECM.Controllers;
using UnityEngine;

namespace ECM.Examples
{
    /// <summary>
    /// 
    /// Example Character Controller.
    /// 
    /// It shows how easy is to create a custom controller for a player controlled characters.
    /// It inherits from 'BaseCharacterController' and adds 'Ethan' character animation code.
    /// 
    /// </summary>

    public sealed class EthanCharacterController : BaseCharacterController
    {
        /// <summary>
        /// Implements 'BaseAnimatorCharacterController' Animate method.
        /// 
        /// This shows how to handle your characters' animation states using the Animate method.
        /// The use of this method is optional, for example you can use a separate script to manage your
        /// animations completely separate of movement controller.
        /// </summary>

        protected override void Animate()
        {
            // If there is no animator, return

            if (animator == null)
                return;

            // Compute move vector in local space

            var move = transform.InverseTransformDirection(moveDirection);

            // Update the animator parameters

            var forwardAmount = move.z;

            animator.SetFloat("Forward", forwardAmount, 0.1f, Time.deltaTime);
            animator.SetFloat("Turn", Mathf.Atan2(move.x, move.z), 0.1f, Time.deltaTime);

            animator.SetBool("OnGround", movement.isGrounded);

            if (!movement.isGrounded)
                animator.SetFloat("Jump", movement.velocity.y, 0.1f, Time.deltaTime);

            // Calculate which leg is behind, so as to leave that leg trailing in the jump animation
            // (This code is reliant on the specific run cycle offset in our animations,
            // and assumes one leg passes the other at the normalized clip times of 0.0 and 0.5)

            var runCycle = Mathf.Repeat(animator.GetCurrentAnimatorStateInfo(0).normalizedTime + 0.2f, 1.0f);
            var jumpLeg = (runCycle < 0.5f ? 1.0f : -1.0f) * forwardAmount;

            if (movement.isGrounded)
                animator.SetFloat("JumpLeg", jumpLeg);
        }
    }
}