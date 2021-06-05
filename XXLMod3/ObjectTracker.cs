using GameManagement;
using ReplayEditor;
using SkaterXL.Data;
using System.Collections.Generic;
using UnityEngine;

namespace XXLMod3
{
    public class RecordedFrame
    {
        public TransformInfo TransformInfo;
        public float Time;

        public RecordedFrame(TransformInfo transformInfo, float time)
        {
            TransformInfo = transformInfo;
            Time = time;
        }
    }

    public class ObjectTracker : MonoBehaviour
    {
        private float nextRecordTime;
        private List<RecordedFrame> recordedFrames;
        private Rigidbody rigidBody;
        private BoxCollider collider;
        private int BufferFrameCount;
        private Animation anim;
        private AnimationClip clip;
        private bool clipUpdated;
        private Vector3 lastPosition;
        private Quaternion lastRotation;

        private void Awake()
        {
            recordedFrames = new List<RecordedFrame>();
            rigidBody = GetComponent<Rigidbody>();
            collider = GetComponent<BoxCollider>();
            BufferFrameCount = Mathf.RoundToInt(ReplaySettings.Instance.FPS * ReplaySettings.Instance.MaxRecordedTime);
        }

        private void Start()
        {
            //ReplayRecorder.Instance.ClearRecordedFrames();
            //ReplayEditorController.Instance.playbackController.OnTimeChanged += PlaybackController_OnTimeChanged;
        }

        //private void PlaybackController_OnTimeChanged(float time, float timeScale)
        //{
        //    var prevFrameIndex = ReplayEditorController.Instance.playbackController.prevFrameIndex;
        //    var previousFrameTime = ReplayEditorController.Instance.playbackController.ClipFrames[prevFrameIndex + 1].time;
        //    var previousFrame = recordedFrames.OrderByDescending(x => x.time).FirstOrDefault(x => x.time <= previousFrameTime);
        //    var currentFrame = recordedFrames.OrderBy(x => x.time).FirstOrDefault(x => x.time >= previousFrameTime);
        //    if (previousFrameTime < recordedFrames.FirstOrDefault().time) return;

        //    var firstFrame = recordedFrames[0];
        //    var lastFrame = recordedFrames[recordedFrames.Count - 1];

        //    Main.modEntry.Logger.Log(firstFrame.time.ToString() + "lastframe:" + lastFrame.time.ToString());

        //    Animation anim = gameObject.GetComponent<Animation>();

        //   if(anim == null)
        //    {
        //        anim = gameObject.AddComponent<Animation>();
        //    }

        //    var clip = new AnimationClip();
        //    clip.legacy = true;
        //    clip.name = "CubeReplayTest";

        //    clip.SetCurve("", typeof(Transform), "localPosition.x", AnimationCurve.Linear(previousFrame.time, previousFrame.transformInfo.position.x, currentFrame.time, currentFrame.transformInfo.position.x));
        //    clip.SetCurve("", typeof(Transform), "localPosition.y", AnimationCurve.Linear(previousFrame.time, previousFrame.transformInfo.position.y, currentFrame.time, currentFrame.transformInfo.position.y));
        //    clip.SetCurve("", typeof(Transform), "localPosition.z", AnimationCurve.Linear(previousFrame.time, previousFrame.transformInfo.position.z, currentFrame.time, currentFrame.transformInfo.position.z));

        //    clip.SetCurve("", typeof(Transform), "localRotation.x", AnimationCurve.Linear(previousFrame.time, previousFrame.transformInfo.rotation.x, currentFrame.time, currentFrame.transformInfo.rotation.x));
        //    clip.SetCurve("", typeof(Transform), "localRotation.y", AnimationCurve.Linear(previousFrame.time, previousFrame.transformInfo.rotation.y, currentFrame.time, currentFrame.transformInfo.rotation.y));
        //    clip.SetCurve("", typeof(Transform), "localRotation.z", AnimationCurve.Linear(previousFrame.time, previousFrame.transformInfo.rotation.z, currentFrame.time, currentFrame.transformInfo.rotation.z));
        //    clip.SetCurve("", typeof(Transform), "localRotation.w", AnimationCurve.Linear(previousFrame.time, previousFrame.transformInfo.rotation.w, currentFrame.time, currentFrame.transformInfo.rotation.w));

        //    anim.AddClip(clip, clip.name);
        //    var state = anim[clip.name];

        //    if(!anim.isPlaying && ReplayEditorController.Instance.playbackController.TimeScale != 0.0)
        //    {
        //        anim.Play(clip.name);
        //    }

        //    if (Mathf.Abs(state.time - ReplayEditorController.Instance.playbackController.CurrentTime) > 0.01f)
        //    {
        //        state.time = ReplayEditorController.Instance.playbackController.CurrentTime;
        //    }

        //    state.speed = ReplayEditorController.Instance.playbackController.TimeScale;
        //}

        private void RecordFrame()
        {
            if (nextRecordTime > PlayTime.time)
            {
                return;
            }
            if(nextRecordTime < PlayTime.time - 1f)
            {
                nextRecordTime = PlayTime.time + 1f / 30f;
            }
            else
            {
                nextRecordTime += 1f / 30f;
            }

            RecordedFrame tempRecordedFrame;
            if (recordedFrames.Count >= BufferFrameCount)
            {
                tempRecordedFrame = recordedFrames[0];
                recordedFrames.RemoveAt(0);
                tempRecordedFrame.Time = PlayTime.time;
            }
            else
            {
                tempRecordedFrame = new RecordedFrame(new TransformInfo(), PlayTime.time);
            }

            if (tempRecordedFrame.TransformInfo == null)
            {
                tempRecordedFrame.TransformInfo = new TransformInfo(transform, Space.Self);
            }

            tempRecordedFrame.TransformInfo.position = transform.localPosition;
            tempRecordedFrame.TransformInfo.rotation = transform.localRotation;
            tempRecordedFrame.Time = PlayTime.time;
            recordedFrames.Add(tempRecordedFrame);
        }

        private void Update()
        {
            if (GameStateMachine.Instance.CurrentState.GetType() == typeof(PlayState))
            {
                if (rigidBody.isKinematic)
                {
                    if(anim != null && anim.isPlaying)
                    {
                        anim.Stop();
                    }
                    transform.localPosition = lastPosition;
                    transform.localRotation = lastRotation;
                    rigidBody.isKinematic = false;
                    collider.isTrigger = false;
                    clipUpdated = false;
                }

                RecordFrame();
            }
            if (GameStateMachine.Instance.CurrentState.GetType() == typeof(ReplayState))
            {
                if (!rigidBody.isKinematic)
                {
                    lastPosition = transform.localPosition;
                    lastRotation = transform.localRotation;
                    rigidBody.isKinematic = true;
                    collider.isTrigger = true;
                }

                anim = gameObject.GetComponent<Animation>();

                if (anim == null)
                {
                    anim = gameObject.AddComponent<Animation>();
                }

                if (!clip || !clipUpdated)
                {
                    clip = new AnimationClip();
                    clip.legacy = true;
                    clip.name = $"{gameObject.name}";

                    AnimationCurve curve_pos_x = new AnimationCurve();
                    AnimationCurve curve_pos_y = new AnimationCurve();
                    AnimationCurve curve_pos_z = new AnimationCurve();
                    AnimationCurve curve_rot_x = new AnimationCurve();
                    AnimationCurve curve_rot_y = new AnimationCurve();
                    AnimationCurve curve_rot_z = new AnimationCurve();
                    AnimationCurve curve_rot_w = new AnimationCurve();

                    foreach(RecordedFrame frame in recordedFrames)
                    {
                        curve_pos_x.AddKey(frame.Time, frame.TransformInfo.position.x);
                        curve_pos_y.AddKey(frame.Time, frame.TransformInfo.position.y);
                        curve_pos_z.AddKey(frame.Time, frame.TransformInfo.position.z);
                        curve_rot_x.AddKey(frame.Time, frame.TransformInfo.rotation.x);
                        curve_rot_y.AddKey(frame.Time, frame.TransformInfo.rotation.y);
                        curve_rot_z.AddKey(frame.Time, frame.TransformInfo.rotation.z);
                        curve_rot_w.AddKey(frame.Time, frame.TransformInfo.rotation.w);
                    }

                    clip.SetCurve("", typeof(Transform), "localPosition.x", curve_pos_x);
                    clip.SetCurve("", typeof(Transform), "localPosition.y", curve_pos_y);
                    clip.SetCurve("", typeof(Transform), "localPosition.z", curve_pos_z);

                    clip.SetCurve("", typeof(Transform), "localRotation.x", curve_rot_x);
                    clip.SetCurve("", typeof(Transform), "localRotation.y", curve_rot_y);
                    clip.SetCurve("", typeof(Transform), "localRotation.z", curve_rot_z);
                    clip.SetCurve("", typeof(Transform), "localRotation.w", curve_rot_w);
                    clipUpdated = true;
                }

                anim.AddClip(clip, clip.name);
                anim.animatePhysics = true;

                var state = anim[clip.name];

                if (!anim.isPlaying && ReplayEditorController.Instance.playbackController.TimeScale != 0.0)
                {
                    anim.Play(clip.name);
                }

                state.time = ReplayEditorController.Instance.playbackController.CurrentTime;
                state.speed = ReplayEditorController.Instance.playbackController.TimeScale;
            }
        }
    }
}
