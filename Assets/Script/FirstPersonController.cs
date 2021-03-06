using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Utility;
using Random = UnityEngine.Random;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace UnityStandardAssets.Characters.FirstPerson
{
    [RequireComponent(typeof (CharacterController))]
    [RequireComponent(typeof (AudioSource))]
    public class FirstPersonController : MonoBehaviour
    {
        public int m_DataIntegrity;
        public GameObject[] m_GeneArray;
        [SerializeField] private bool m_IsWalking;
        [SerializeField] private float m_WalkSpeed;
        [SerializeField] private float m_RunSpeed;
        [SerializeField] [Range(0f, 1f)] private float m_RunstepLenghten;
        [SerializeField] private float m_JumpSpeed;
        [SerializeField] private float m_StickToGroundForce;
        [SerializeField] private float m_GravityMultiplier;
        [SerializeField] private MouseLook m_MouseLook;
        [SerializeField] private bool m_UseFovKick;
        [SerializeField] private FOVKick m_FovKick = new FOVKick();
        [SerializeField] private bool m_UseHeadBob;
        [SerializeField] private CurveControlledBob m_HeadBob = new CurveControlledBob();
        [SerializeField] private LerpControlledBob m_JumpBob = new LerpControlledBob();
        [SerializeField] private float m_StepInterval;
        [SerializeField] private AudioClip[] m_FootstepSounds;    // an array of footstep sounds that will be randomly selected from.
        [SerializeField] private AudioClip m_JumpSound;           // the sound played when character leaves the ground.
        [SerializeField] private AudioClip m_LandSound;           // the sound played when character touches back on ground.
        [SerializeField] private AudioClip m_Bibi;
        public Slider healthBar;

        private Camera m_Camera;
        private bool m_Jump;
        private float m_YRotation;
        private Vector2 m_Input;
        private Vector3 m_MoveDir = Vector3.zero;
        private CharacterController m_CharacterController;
        private CollisionFlags m_CollisionFlags;
        private bool m_PreviouslyGrounded;
        private Vector3 m_OriginalCameraPosition;
        private float m_StepCycle;
        private float m_NextStep;
        private bool m_Jumping;
        private AudioSource m_AudioSource;
        private GameObject m_UI;
        int tick=0;
        int savedInt=-1;
        GameObject pip;
        GameObject menu;
        GameObject tutor;
        bool tutorShown;
        bool pause;


        // Use this for initialization
        private void Start()
        {
            m_GeneArray = GameObject.FindGameObjectsWithTag("Target");
            m_CharacterController = GetComponent<CharacterController>();
            m_Camera = Camera.main;
            m_OriginalCameraPosition = m_Camera.transform.localPosition;
            m_FovKick.Setup(m_Camera);
            m_HeadBob.Setup(m_Camera, m_StepInterval);
            m_StepCycle = 0f;
            m_NextStep = m_StepCycle/2f;
            m_Jumping = false;
            m_AudioSource = GetComponent<AudioSource>();
			m_MouseLook.Init(transform , m_Camera.transform);
            m_UI = GameObject.Find("Canvas");
            menu = GameObject.Find("Panel");
            pip = GameObject.Find("Pip");
            pip.SetActive(false);
            menu.SetActive(false);
            setBar();
            pause = false;
            tutor = GameObject.Find("Tutor");
            tutorShown = true;
        }

        private void setBar()
        {
            healthBar.value = m_DataIntegrity / 10f;
        }

        // Update is called once per frame
        private void Update()
        {
            //setbar every frame
            setBar();
            //pause
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                if (!pause)
                {
                    menu.SetActive(true);
                    m_MouseLook.SetCursorLock(false);
                    pause = true;
                }
                else
                {
                    menu.SetActive(false);
                    m_MouseLook.SetCursorLock(true);
                    pause = false;
                }
            }
            
            //set check point
            if (Input.GetKeyUp(KeyCode.E))
            {
                pip.SetActive(true);
                pip.transform.position = transform.position;
                savedInt = m_DataIntegrity;
            }

            //send back to last check point.
            if (Input.GetKeyDown(KeyCode.R)&&savedInt!=-1)
            {
                transform.position = pip.transform.position;
                m_DataIntegrity = savedInt;
                setBar();
            }

            //showing help
            if (Input.GetKeyDown(KeyCode.H))
            {
                if (tutorShown)
                {
                    tutor.SetActive(false);
                    tutorShown = false;
                }
                else
                {
                    tutor.SetActive(true);
                    tutorShown = true;
                }
            }

            // code for showing minimap
            if (!Input.GetKey(KeyCode.Q))
                RotateView();
            if (Input.GetKeyDown(KeyCode.Q))
            {
                //m_UI.SetActive(true);
                m_MouseLook.SetCursorLock(false);
            }
            if (Input.GetKeyUp(KeyCode.Q))
            {
                //m_UI.SetActive(false);
                m_MouseLook.SetCursorLock(true);
            }

            // the jump state needs to read here to make sure it is not missed
            if (!m_Jump)
            {
                m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
            }

            if (!m_PreviouslyGrounded && m_CharacterController.isGrounded)
            {
                StartCoroutine(m_JumpBob.DoBobCycle());
                PlayLandingSound();
                m_MoveDir.y = 0f;
                m_Jumping = false;
            }
            if (!m_CharacterController.isGrounded && !m_Jumping && m_PreviouslyGrounded)
            {
                m_MoveDir.y = 0f;
            }

            m_PreviouslyGrounded = m_CharacterController.isGrounded;

            // update voice
            GameObject nearest = m_GeneArray[0];
            float dis = Vector3.Distance(m_GeneArray[0].transform.position, transform.position);
            for (int i = 0; i < m_GeneArray.Length; i++)
            {
                float d = Vector3.Distance(m_GeneArray[i].transform.position, transform.position);
                if (d < dis)
                {
                    nearest = m_GeneArray[i];
                    dis = d;
                }
                else
                // change the background noise to 0 if not nearest
                {
                    m_GeneArray[i].GetComponents<AudioSource>()[1].volume = 0;
                }
            }
            AudioSource[] AudioArray = nearest.GetComponents<AudioSource>();
            AudioArray[0].volume = m_DataIntegrity / 10.0f;
            AudioArray[0].dopplerLevel = 5f;
            AudioArray[0].pitch = 1f;
            AudioArray[1].volume = 1 - m_DataIntegrity / 10.0f;
            float interval = dis / 100f * 1.7f + 0.680f; // at which distance the latency is 2s
            float ticks = interval / Time.deltaTime;
            tick++;
            if (tick >= ticks) //if reached tick interval
            {
                tick = 0;
                AudioArray[0].Play();
            }
        }


        private void PlayLandingSound()
        {
            m_AudioSource.clip = m_LandSound;
            m_AudioSource.Play();
            m_NextStep = m_StepCycle + .5f;
        }


        private void FixedUpdate()
        {
            float speed;
            GetInput(out speed);
            // always move along the camera forward as it is the direction that it being aimed at
            Vector3 desiredMove = transform.forward*m_Input.y + transform.right*m_Input.x;
            Vector3 step = new Vector3(desiredMove.x*speed,0,desiredMove.z*speed);
            step = (step.normalized) * 1.5f;
            // get a normal for the surface that is being touched to move along it
            RaycastHit hitInfo;
            Physics.SphereCast(transform.position, m_CharacterController.radius, Vector3.down, out hitInfo,
                               m_CharacterController.height/2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
            desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

            m_MoveDir.x = desiredMove.x*speed;
            m_MoveDir.z = desiredMove.z*speed;
           

            if (m_CharacterController.isGrounded)
            {
                m_MoveDir.y = -m_StickToGroundForce;

                if (/*m_Jump*/false)
                {
                    m_MoveDir.y = m_JumpSpeed;
                    PlayJumpSound();
                    m_Jump = false;
                    m_Jumping = true;
                }
            }
            else
            {
                m_MoveDir += Physics.gravity*m_GravityMultiplier*Time.fixedDeltaTime;
            }
            
            m_CollisionFlags = m_CharacterController.Move(m_MoveDir*Time.fixedDeltaTime);

            //turn on wallpassing mode
            if ((m_CollisionFlags & CollisionFlags.Sides) != 0 && Input.GetMouseButton(0) && m_DataIntegrity>0)
            {
                Vector3 curPos=transform.position;
                curPos += step;
                if (!Physics.CheckSphere(curPos, 0.5f, ~(1 << 8))){
                    //check if the colliders overlapping after tranformation
                    transform.position = curPos;
                    m_DataIntegrity--;
                    //Debug.Log(m_DataIntegrity + "/10");
                    setBar();
                }
            }

            ProgressStepCycle(speed);
            UpdateCameraPosition(speed);

            m_MouseLook.UpdateCursorLock();

            
        }


        private void PlayJumpSound()
        {
            m_AudioSource.clip = m_JumpSound;
            m_AudioSource.Play();
        }


        private void ProgressStepCycle(float speed)
        {
            if (m_CharacterController.velocity.sqrMagnitude > 0 && (m_Input.x != 0 || m_Input.y != 0))
            {
                m_StepCycle += (m_CharacterController.velocity.magnitude + (speed*(m_IsWalking ? 1f : m_RunstepLenghten)))*
                             Time.fixedDeltaTime;
            }

            if (!(m_StepCycle > m_NextStep))
            {
                return;
            }

            m_NextStep = m_StepCycle + m_StepInterval;

            PlayFootStepAudio();
        }


        private void PlayFootStepAudio()
        {
            if (!m_CharacterController.isGrounded)
            {
                return;
            }
            // pick & play a random footstep sound from the array,
            // excluding sound at index 0
            int n = Random.Range(1, m_FootstepSounds.Length);
            m_AudioSource.clip = m_FootstepSounds[n];
            m_AudioSource.PlayOneShot(m_AudioSource.clip);
            // move picked sound to index 0 so it's not picked next time
            m_FootstepSounds[n] = m_FootstepSounds[0];
            m_FootstepSounds[0] = m_AudioSource.clip;
        }


        private void UpdateCameraPosition(float speed)
        {
            Vector3 newCameraPosition;
            if (!m_UseHeadBob)
            {
                return;
            }
            if (m_CharacterController.velocity.magnitude > 0 && m_CharacterController.isGrounded)
            {
                m_Camera.transform.localPosition =
                    m_HeadBob.DoHeadBob(m_CharacterController.velocity.magnitude +
                                      (speed*(m_IsWalking ? 1f : m_RunstepLenghten)));
                newCameraPosition = m_Camera.transform.localPosition;
                newCameraPosition.y = m_Camera.transform.localPosition.y - m_JumpBob.Offset();
            }
            else
            {
                newCameraPosition = m_Camera.transform.localPosition;
                newCameraPosition.y = m_OriginalCameraPosition.y - m_JumpBob.Offset();
            }
            m_Camera.transform.localPosition = newCameraPosition;
        }


        private void GetInput(out float speed)
        {
            // Read input
            float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
            float vertical = CrossPlatformInputManager.GetAxis("Vertical");

            bool waswalking = m_IsWalking;

#if !MOBILE_INPUT
            // On standalone builds, walk/run speed is modified by a key press.
            // keep track of whether or not the character is walking or running
            m_IsWalking = !Input.GetKey(KeyCode.LeftShift);
#endif
            //disable running
            m_IsWalking = true;

            // set the desired speed to be walking or running
            speed = m_IsWalking ? m_WalkSpeed : m_RunSpeed;
            m_Input = new Vector2(horizontal, vertical);

            // normalize input if it exceeds 1 in combined length:
            if (m_Input.sqrMagnitude > 1)
            {
                m_Input.Normalize();
            }

            // handle speed change to give an fov kick
            // only if the player is going to a run, is running and the fovkick is to be used
            if (m_IsWalking != waswalking && m_UseFovKick && m_CharacterController.velocity.sqrMagnitude > 0)
            {
                StopAllCoroutines();
                StartCoroutine(!m_IsWalking ? m_FovKick.FOVKickUp() : m_FovKick.FOVKickDown());
            }
        }


        private void RotateView()
        {
            m_MouseLook.LookRotation (transform, m_Camera.transform);
        }


        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            Rigidbody body = hit.collider.attachedRigidbody;
            //dont move the rigidbody if the character is on top of it
            if (m_CollisionFlags == CollisionFlags.Below)
            {
                return;
            }

            if (body == null || body.isKinematic)
            {
                return;
            }
            body.AddForceAtPosition(m_CharacterController.velocity*0.1f, hit.point, ForceMode.Impulse);
        }
    }
}
