﻿using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniGLTF;
using VRM;
using VRMLoader;
using B83.Win32;
using TriLibCore;
using RootMotion.FinalIK;
using RootMotion.Dynamics;
using Battlehub.RTHandles;

public class LoadFile : MonoBehaviour
{
    [SerializeField] GameObject LoadConfirmModal;
    [SerializeField] Canvas Canvas;
    [SerializeField] GameObject MarkerR;
    [SerializeField] GameObject MarkerG;
    [SerializeField] GameObject MarkerB;
    [SerializeField] GameObject MarkerC;
    [SerializeField] GameObject MarkerM;
    [SerializeField] GameObject MarkerP;
    [SerializeField] GameObject Photo;
    [SerializeField] GameObject DandD;
    [SerializeField] SelectVRM SelectVRM;
    [SerializeField] GameObject Camera;
    [SerializeField] GameObject Light;
    [SerializeField] RuntimeSceneComponent RuntimeSceneComponent;
    [SerializeField] GameObject TEST;
    [SerializeField] LinkBlendShape LinkBlendShape;
    [SerializeField] LinkHand LinkHand;
    [SerializeField] Toggle Move;

    List<HumanBodyBones> PrimalBones;
    AssetBundle AssetBundle;

    GameObject Background;

    int LoadCount = 0;
    int LoadCountMax = -1;
    string SceneText;

    void OnEnable()
    {
        UnityDragAndDropHook.InstallHook();
        UnityDragAndDropHook.OnDroppedFiles += OnFiles;
    }

    void OnDisable()
    {
        UnityDragAndDropHook.UninstallHook();
    }

    void OnFiles(List<string> aFiles, POINT aPos)
    {
        foreach (string path in aFiles)
        {
            var ext = Path.GetExtension(path);
            if (string.Compare(ext, ".vrm", true) == 0)
            {
                VRM(path);
            }
            else if (string.Compare(ext, ".glb", true) == 0)
            {
                GLB(path);
            }
            else if (string.Compare(ext, ".jpg", true) == 0 || string.Compare(ext, ".png", true) == 0)
            {
                IMG(path);
            }
            else if (string.Compare(ext, ".ab", true) == 0)
            {
                AB(path);
            }
            else if (string.Compare(ext, ".txt", true) == 0)
            {
                TXT(path);
            }
            else
            {
                TriLib(path);
            }
        }
    }

    void Start()
    {
        PrimalBones = new List<HumanBodyBones>();
        PrimalBones.Add(HumanBodyBones.Head);
        PrimalBones.Add(HumanBodyBones.Neck);
        PrimalBones.Add(HumanBodyBones.Hips);
        PrimalBones.Add(HumanBodyBones.Spine);
        PrimalBones.Add(HumanBodyBones.Chest);
        PrimalBones.Add(HumanBodyBones.UpperChest);
        PrimalBones.Add(HumanBodyBones.LeftUpperArm);
        PrimalBones.Add(HumanBodyBones.LeftLowerArm);
        PrimalBones.Add(HumanBodyBones.LeftHand);
        PrimalBones.Add(HumanBodyBones.RightUpperArm);
        PrimalBones.Add(HumanBodyBones.RightLowerArm);
        PrimalBones.Add(HumanBodyBones.RightHand);
        PrimalBones.Add(HumanBodyBones.LeftUpperLeg);
        PrimalBones.Add(HumanBodyBones.LeftLowerLeg);
        PrimalBones.Add(HumanBodyBones.LeftFoot);
        PrimalBones.Add(HumanBodyBones.RightUpperLeg);
        PrimalBones.Add(HumanBodyBones.RightLowerLeg);
        PrimalBones.Add(HumanBodyBones.RightFoot);

#if UNITY_EDITOR
        VRM(@"D:\play\UnityProject\VRM_DollPlayPCverTestData\AoUni.vrm");
#else
        TEST.SetActive(false);
#endif
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.PageUp))
        {
            DestroyJointAndRigid();
            SetRagDoll();
            DisableGravity();
        }
        if (Input.GetKey(KeyCode.PageDown))
        {
            DestroyJointAndRigid();
            SetRagDoll();
        }
        if (LoadCountMax == LoadCount)
        {
            LoadCount = 0;
            LoadCountMax = -1;

            var line = SceneText.Split('\n');
            var objects = Array.FindAll(FindObjectsOfType<GameObject>(), (item) => item.transform.parent == null);
            for (int i = 0; i < line.Length; i++)
            {
                var lineCSV = line[i].Split(',');
                var item = lineCSV[0];
                if (item != "" && item != "Camera" && item != "Light")
                {
                    foreach (var obj in objects)
                    {
                        var saveSceneTarget = obj.GetComponent<SaveSceneTarget>();
                        if (saveSceneTarget != null && saveSceneTarget.Path == item)
                        {
                            obj.transform.position = GetVector3(lineCSV, 1);
                            obj.transform.eulerAngles = GetVector3(lineCSV, 4);
                            obj.transform.localScale = GetVector3(lineCSV, 7);
                            break;
                        }
                    }
                }
            }
        }
    }

    public void GLB_TEST()
    {
        GLB(@"D:\play\UnityProject\VRM_DollPlayPCverTestData\sample.glb");
    }

    public void AB_TEST()
    {
        AB(@"D:\play\UnityProject\VRM_DollPlayPCverTestData\crs_full.ab");
    }

    public void TXT_TEST()
    {
        TXT(@"D:\play\UnityProject\VRM_DollPlayPCverTestData\scene.txt");
    }

    public void PNG_TEST()
    {
        IMG(@"D:\play\UnityProject\VRM_DollPlayPCverTestData\pose.png");
    }

    public void TriLib_TEST()
    {
        TriLib(@"D:\play\UnityProject\VRM_DollPlayPCverTestData\Model_scooter.fbx");
    }

    void VRM(string path)
    {
        var data = new AutoGltfFileParser(path).Parse();
        var vrm = new VRMData(data);
        using (var loader = new VRMImporterContext(vrm))
        {
            var meta = loader.ReadMeta(true);
            var modal = Instantiate(LoadConfirmModal, Canvas.transform);
            var ui = modal.GetComponentInChildren<VRMPreviewUI>();
            ui.setMeta(meta);
            ui.setLoadable(true);
            ui.m_ok.onClick.AddListener(() => LoadAsync(loader, path));
        }
    }

    void GLB(string path)
    {
        var data = new AutoGltfFileParser(path).Parse();
        using (var loader = new ImporterContext(data))
        {
            var markerM = Instantiate(MarkerM).transform;
            markerM.name = "GLB_Root";
            markerM.position = new Vector3(0, -1, 0);
            markerM.gameObject.AddComponent<SaveSceneTarget>().Path = path;

            var instance = loader.Load();
            instance.Root.transform.position = new Vector3(0, -1, 0);
            instance.Root.transform.parent = markerM;
            instance.ShowMeshes();

            RuntimeSceneComponent.Selection.activeGameObject = markerM.gameObject;

            LoadCount++;
        }

        DandD.SetActive(false);
    }

    void LoadAsync(VRMImporterContext context, string path)
    {
        var instance = context.Load();
        OnLoaded(instance, path);
    }

    void OnLoaded(RuntimeGltfInstance instance, string path)
    {
        instance.ShowMeshes();
        SetVRM(instance.Root, path);
        LoadCount++;
    }

    public void SetVRM(GameObject root, string path)
    {
        root.transform.position = new Vector3(0, -1, 0);

        // 根本
        var markerR = Instantiate(MarkerR).transform;
        markerR.name = "Root";
        markerR.position = root.transform.position;
        markerR.GetComponent<Marker>().List = new List<Transform>();
        markerR.gameObject.AddComponent<SaveSceneTarget>().Path = path;
        root.transform.parent = markerR;

        // 関節
        var anim = root.GetComponent<Animator>();
        var hips = anim.GetBoneTransform(HumanBodyBones.Hips);
        var transforms = root.GetComponentsInChildren<Transform>();
        foreach (var t in transforms)
        {
            var original = t == hips ? MarkerC : MarkerB;
            var marker = Instantiate(original).transform;
            marker.gameObject.SetActive(false);
            marker.position = t.position;
            marker.parent = t.parent;
            marker.GetComponent<LinkRotation>().Target = t;
            markerR.GetComponent<Marker>().List.Add(marker);
            foreach (var bone in PrimalBones)
            {
                if (t == anim.GetBoneTransform(bone))
                {
                    marker.GetComponent<LinkRotation>().IsPrimal = true;
                    marker.gameObject.SetActive(true);
                    break;
                }
            }
        }

        // PrimalBones順に並び替え（旧版互換目的）
        var markerList = markerR.GetComponent<Marker>().List;
        for (int i = 0; i < PrimalBones.Count; i++)
        {
            var bone = anim.GetBoneTransform(PrimalBones[i]);
            if (bone != null)
            {
                foreach (var marker in markerList)
                {
                    var target = marker.GetComponent<LinkRotation>().Target;
                    if (bone == target)
                    {
                        markerList.Remove(marker);
                        markerList.Insert(i, marker);
                        break;
                    }
                }
            }
            else
            {
                // ポーズ保存のため存在しない骨もダミーとして追加
                var marker = Instantiate(MarkerB).transform;
                marker.parent = markerR;
                marker.GetComponent<LinkRotation>().IsPrimal = true;
                markerList.Insert(i, marker);
            }
        }

        // 視線
        var markerG = Instantiate(MarkerG).transform;
        var head = anim.GetBoneTransform(HumanBodyBones.Head);
        markerG.position = head.position - new Vector3(0, 0, -0.5f);
        markerG.parent = head;
        root.GetComponent<VRMLookAtHead>().Target = markerG;

        // VRIK
        var vrik = root.AddComponent<VRIK>();
        vrik.enabled = false;

        vrik.solver.spine.headTarget = Instantiate(MarkerB).transform;
        vrik.solver.spine.headTarget.parent = markerR;
        vrik.solver.spine.headTarget.name = "VRIK_Target";
        vrik.solver.spine.headTarget.position = anim.GetBoneTransform(HumanBodyBones.Head).position;
        vrik.solver.spine.headTarget.gameObject.SetActive(false);
        markerR.GetComponent<Marker>().List.Add(vrik.solver.spine.headTarget);

        vrik.solver.spine.pelvisTarget = Instantiate(MarkerB).transform;
        vrik.solver.spine.pelvisTarget.parent = markerR;
        vrik.solver.spine.pelvisTarget.name = "VRIK_Target";
        vrik.solver.spine.pelvisTarget.position = anim.GetBoneTransform(HumanBodyBones.Hips).position;
        vrik.solver.spine.pelvisTarget.gameObject.SetActive(false);
        vrik.solver.spine.pelvisPositionWeight = 1;
        vrik.solver.spine.pelvisRotationWeight = 1;
        vrik.solver.plantFeet = false;
        markerR.GetComponent<Marker>().List.Add(vrik.solver.spine.pelvisTarget);

        vrik.solver.leftArm.target = Instantiate(MarkerB).transform;
        vrik.solver.leftArm.target.parent = markerR;
        vrik.solver.leftArm.target.name = "VRIK_Target";
        vrik.solver.leftArm.target.position = anim.GetBoneTransform(HumanBodyBones.LeftHand).position;
        vrik.solver.leftArm.target.gameObject.SetActive(false);
        markerR.GetComponent<Marker>().List.Add(vrik.solver.leftArm.target);

        vrik.solver.rightArm.target = Instantiate(MarkerB).transform;
        vrik.solver.rightArm.target.parent = markerR;
        vrik.solver.rightArm.target.name = "VRIK_Target";
        vrik.solver.rightArm.target.position = anim.GetBoneTransform(HumanBodyBones.RightHand).position;
        vrik.solver.rightArm.target.gameObject.SetActive(false);
        markerR.GetComponent<Marker>().List.Add(vrik.solver.rightArm.target);

        vrik.solver.leftLeg.target = Instantiate(MarkerB).transform;
        vrik.solver.leftLeg.target.parent = markerR;
        vrik.solver.leftLeg.target.name = "VRIK_Target";
        vrik.solver.leftLeg.target.position = anim.GetBoneTransform(HumanBodyBones.LeftFoot).position;
        vrik.solver.leftLeg.target.gameObject.SetActive(false);
        vrik.solver.leftLeg.positionWeight = 1;
        vrik.solver.leftLeg.rotationWeight = 1;
        markerR.GetComponent<Marker>().List.Add(vrik.solver.leftLeg.target);

        vrik.solver.rightLeg.target = Instantiate(MarkerB).transform;
        vrik.solver.rightLeg.target.parent = markerR;
        vrik.solver.rightLeg.target.name = "VRIK_Target";
        vrik.solver.rightLeg.target.position = anim.GetBoneTransform(HumanBodyBones.RightFoot).position;
        vrik.solver.rightLeg.target.gameObject.SetActive(false);
        vrik.solver.rightLeg.positionWeight = 1;
        vrik.solver.rightLeg.rotationWeight = 1;
        markerR.GetComponent<Marker>().List.Add(vrik.solver.rightLeg.target);

        var leftKnee = Instantiate(MarkerP).transform;
        leftKnee.name = "VRIK_Target";
        leftKnee.eulerAngles = new Vector3(0, -50, 0);
        leftKnee.position = anim.GetBoneTransform(HumanBodyBones.LeftLowerLeg).position;
        leftKnee.parent = anim.GetBoneTransform(HumanBodyBones.LeftLowerLeg);
        leftKnee.gameObject.SetActive(false);
        leftKnee.GetComponent<LinkSwivelOffset>().VRIK = vrik;
        leftKnee.GetComponent<LinkSwivelOffset>().KE = LinkSwivelOffset.Regio.Knee;
        leftKnee.GetComponent<LinkSwivelOffset>().LR = LinkSwivelOffset.Side.L;
        markerR.GetComponent<Marker>().List.Add(leftKnee);

        var rightKnee = Instantiate(MarkerP).transform;
        rightKnee.name = "VRIK_Target";
        rightKnee.eulerAngles = new Vector3(0, 50, 0);
        rightKnee.position = anim.GetBoneTransform(HumanBodyBones.RightLowerLeg).position;
        rightKnee.parent = anim.GetBoneTransform(HumanBodyBones.RightLowerLeg);
        rightKnee.gameObject.SetActive(false);
        rightKnee.GetComponent<LinkSwivelOffset>().VRIK = vrik;
        rightKnee.GetComponent<LinkSwivelOffset>().KE = LinkSwivelOffset.Regio.Knee;
        rightKnee.GetComponent<LinkSwivelOffset>().LR = LinkSwivelOffset.Side.R;
        markerR.GetComponent<Marker>().List.Add(rightKnee);

        var leftElbow = Instantiate(MarkerP).transform;
        leftElbow.name = "VRIK_Target";
        leftElbow.eulerAngles = new Vector3(0, 0, 0);
        leftElbow.position = anim.GetBoneTransform(HumanBodyBones.LeftLowerArm).position;
        leftElbow.parent = anim.GetBoneTransform(HumanBodyBones.LeftLowerArm);
        leftElbow.gameObject.SetActive(false);
        leftElbow.GetComponent<LinkSwivelOffset>().VRIK = vrik;
        leftElbow.GetComponent<LinkSwivelOffset>().KE = LinkSwivelOffset.Regio.Elbow;
        leftElbow.GetComponent<LinkSwivelOffset>().LR = LinkSwivelOffset.Side.L;
        markerR.GetComponent<Marker>().List.Add(leftElbow);

        var rightElbow = Instantiate(MarkerP).transform;
        rightElbow.name = "VRIK_Target";
        rightElbow.eulerAngles = new Vector3(0, 0, 0);
        rightElbow.position = anim.GetBoneTransform(HumanBodyBones.RightLowerArm).position;
        rightElbow.parent = anim.GetBoneTransform(HumanBodyBones.RightLowerArm);
        rightElbow.gameObject.SetActive(false);
        rightElbow.GetComponent<LinkSwivelOffset>().VRIK = vrik;
        rightElbow.GetComponent<LinkSwivelOffset>().KE = LinkSwivelOffset.Regio.Elbow;
        rightElbow.GetComponent<LinkSwivelOffset>().LR = LinkSwivelOffset.Side.R;
        markerR.GetComponent<Marker>().List.Add(rightElbow);

        // ラグドール化により一部メッシュが非表示になりやすいので対策（主にVRoid）
        var meshs = root.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (var mesh in meshs)
        {
            mesh.updateWhenOffscreen = true;
        }

        // ABファイルから適用されるアニメーションで原点移動を有効にしたい
        anim.applyRootMotion = true;

        DandD.SetActive(false);

        RuntimeSceneComponent.Selection.activeGameObject = markerR.gameObject;
        Move.isOn = true;
    }

    public GameObject IMG(string path)
    {
        var tex = new Texture2D(1, 1);
        var img = File.ReadAllBytes(path);

        tex.LoadImage(img);

        // ポーズ画像チェック
        var color = tex.GetPixels();
        var color0r = (byte)(color[0].r * byte.MaxValue);
        var color0g = (byte)(color[0].g * byte.MaxValue);
        var color0b = (byte)(color[0].b * byte.MaxValue);
        var color0a = (byte)(color[0].a * byte.MaxValue);
        if (color0r == 'p' && color0g == 'o' && color0b == 's' && color0a == 'e')
        {
            if (SelectVRM.IsActive)
            {
                for (int i = 0; i < SelectVRM.Marker.List.Count; i++)
                {
                    // ポーズ適用
                    var index = i * 3 + 1;
                    byte[] byte_x = { (byte)(color[index + 0].r * byte.MaxValue), (byte)(color[index + 0].g * byte.MaxValue), (byte)(color[index + 0].b * byte.MaxValue), (byte)(color[index + 0].a * byte.MaxValue) };
                    byte[] byte_y = { (byte)(color[index + 1].r * byte.MaxValue), (byte)(color[index + 1].g * byte.MaxValue), (byte)(color[index + 1].b * byte.MaxValue), (byte)(color[index + 1].a * byte.MaxValue) };
                    byte[] byte_z = { (byte)(color[index + 2].r * byte.MaxValue), (byte)(color[index + 2].g * byte.MaxValue), (byte)(color[index + 2].b * byte.MaxValue), (byte)(color[index + 2].a * byte.MaxValue) };
                    var float_x = BitConverter.ToSingle(byte_x, 0);
                    var float_y = BitConverter.ToSingle(byte_y, 0);
                    var float_z = BitConverter.ToSingle(byte_z, 0);
                    if (!float.IsNaN(float_x) && !float.IsNaN(float_y) && !float.IsNaN(float_z) && SelectVRM.Marker.List[i].gameObject.name != "VRIK_Target")
                    {
                        SelectVRM.Marker.List[i].localEulerAngles = new Vector3(float_x, float_y, float_z);
                    }
                }
                // 指
                {
                    var index = 512 * 1;
                    var start = (int)HumanBodyBones.LeftThumbProximal;
                    var end = (int)HumanBodyBones.RightLittleDistal;
                    for (var i = start; i <= end; i++)
                    {
                        var v3 = ColorToVector3(color, index + i * 3, 1);
                        var tf = SelectVRM.Animator.GetBoneTransform((HumanBodyBones)i);
                        tf.localEulerAngles = v3;
                    }
                    LinkHand.GetFingerAngle();
                }
                // ブレンドシェイプ
                {
                    var index = 512 * 2;
                    var clips = SelectVRM.Proxy.BlendShapeAvatar.Clips;
                    var values = new Dictionary<BlendShapeKey, float>();
                    for (int i = 0; i < clips.Count; i++)
                    {
                        values.Add(clips[i].Key, ColorToFloat(color[index + i]));
                    }
                    SelectVRM.Proxy.SetValues(values);
                    var ii = 512;
                    var skinneds = SelectVRM.RootMarker.GetComponentsInChildren<SkinnedMeshRenderer>();
                    foreach (var skinned in skinneds)
                    {
                        for (int i = 0; i < skinned.sharedMesh.blendShapeCount; i++)
                        {
                            var value = ColorToFloat(color[index + ii]);
                            skinned.SetBlendShapeWeight(i, value);
                            ii++;
                        }
                    }
                    LinkBlendShape.GetBlendShape();
                }
                // VRIKターゲットの位置と回転を読込
                {
                    var index = 512 * 5;
                    var count = SelectVRM.Marker.List.Count;
                    SelectVRM.Marker.List[count - 10].localPosition = ColorToVector3(color, index - 1);
                    SelectVRM.Marker.List[count - 9].localPosition = ColorToVector3(color, index - 4);
                    SelectVRM.Marker.List[count - 8].localPosition = ColorToVector3(color, index - 7);
                    SelectVRM.Marker.List[count - 7].localPosition = ColorToVector3(color, index - 10);
                    SelectVRM.Marker.List[count - 6].localPosition = ColorToVector3(color, index - 13);
                    SelectVRM.Marker.List[count - 5].localPosition = ColorToVector3(color, index - 16);
                    SelectVRM.Marker.List[count - 10].localEulerAngles = ColorToVector3(color, index - 19);
                    SelectVRM.Marker.List[count - 9].localEulerAngles = ColorToVector3(color, index - 22);
                    SelectVRM.Marker.List[count - 8].localEulerAngles = ColorToVector3(color, index - 25);
                    SelectVRM.Marker.List[count - 7].localEulerAngles = ColorToVector3(color, index - 28);
                    SelectVRM.Marker.List[count - 6].localEulerAngles = ColorToVector3(color, index - 31);
                    SelectVRM.Marker.List[count - 5].localEulerAngles = ColorToVector3(color, index - 34);
                    SelectVRM.Marker.List[count - 4].localEulerAngles = ColorToVector3(color, index - 37);
                    SelectVRM.Marker.List[count - 3].localEulerAngles = ColorToVector3(color, index - 40);
                    SelectVRM.Marker.List[count - 2].localEulerAngles = ColorToVector3(color, index - 43);
                    SelectVRM.Marker.List[count - 1].localEulerAngles = ColorToVector3(color, index - 46);
                }
            }
            return null;
        }

        // 画像オブジェクト生成
        var obj = Instantiate(Photo);
        obj.GetComponent<Renderer>().material.SetTexture("_MainTex", tex);
        obj.GetComponent<Renderer>().material.SetTexture("_ShadeTexture", tex);
        obj.transform.position = new Vector3(0, 0, -1);
        obj.transform.eulerAngles = new Vector3(90, 0, 0);
        obj.gameObject.AddComponent<SaveSceneTarget>().Path = path;

        // アス比
        var scale = obj.transform.localScale;
        scale.z *= (float)tex.height / tex.width;
        obj.transform.localScale = scale * 5;

        // Twitter投稿用パス保持
        obj.GetComponent<ImageFile>().Path = path;

        DandD.SetActive(false);

        LoadCount++;

        return obj;
    }

    Vector3 ColorToVector3(Color[] color, int index, int f = -1)
    {
        byte[] byte_x = { (byte)(color[index + 0 * f].r * byte.MaxValue), (byte)(color[index + 0 * f].g * byte.MaxValue), (byte)(color[index + 0 * f].b * byte.MaxValue), (byte)(color[index + 0 * f].a * byte.MaxValue) };
        byte[] byte_y = { (byte)(color[index + 1 * f].r * byte.MaxValue), (byte)(color[index + 1 * f].g * byte.MaxValue), (byte)(color[index + 1 * f].b * byte.MaxValue), (byte)(color[index + 1 * f].a * byte.MaxValue) };
        byte[] byte_z = { (byte)(color[index + 2 * f].r * byte.MaxValue), (byte)(color[index + 2 * f].g * byte.MaxValue), (byte)(color[index + 2 * f].b * byte.MaxValue), (byte)(color[index + 2 * f].a * byte.MaxValue) };
        var float_x = BitConverter.ToSingle(byte_x, 0);
        var float_y = BitConverter.ToSingle(byte_y, 0);
        var float_z = BitConverter.ToSingle(byte_z, 0);
        var vec3 = Vector3.zero;
        if (!float.IsNaN(float_x) && !float.IsNaN(float_y) && !float.IsNaN(float_z))
        {
            vec3 = new Vector3(float_x, float_y, float_z);
        }
        return vec3;
    }

    float ColorToFloat(Color color)
    {
        byte[] data = { (byte)(color.r * byte.MaxValue), (byte)(color.g * byte.MaxValue), (byte)(color.b * byte.MaxValue), (byte)(color.a * byte.MaxValue) };
        var ret = BitConverter.ToSingle(data, 0);
        return ret;
    }

    void AB(string path)
    {
        if (!SelectVRM.IsActive)
        {
            return;
        }

        if (AssetBundle != null) AssetBundle.Unload(false);
        AssetBundle = AssetBundle.LoadFromFile(path);

        var obj = AssetBundle.LoadAsset<GameObject>("GameObject");
        var ctrl = AssetBundle.LoadAsset<RuntimeAnimatorController>("Controller");
        var inst = Instantiate(obj);

        inst.transform.position = new Vector3(0, -1, 0);
        inst.transform.parent = SelectVRM.RootMarker.transform;
        SelectVRM.Animator.runtimeAnimatorController = ctrl;

        SetLayerRecursively(SelectVRM.RootMarker, LayerMask.NameToLayer("Default"));
        SelectVRM.RootMarker.layer = LayerMask.NameToLayer("UIMaterial");
    }

    /// <summary>
    /// 自分自身を含むすべての子オブジェクトのレイヤーを設定します
    /// </summary>
    /// <remarks>
    /// http://baba-s.hatenablog.com/entry/2014/12/02/100720
    /// </remarks>
    void SetLayerRecursively(GameObject self, int layer)
    {
        self.layer = layer;

        foreach (Transform n in self.transform)
        {
            if (n.gameObject.layer != LayerMask.NameToLayer("UIMaterial"))
            {
                SetLayerRecursively(n.gameObject, layer);
            }
        }
    }

    void TXT(string path)
    {
        var txt = File.ReadAllText(path);
        var line = txt.Split('\n');
        var csv = line[0].Split(',');

        if (csv.Length != 3)
        {
            // シーン読込
            LoadCount = 0;
            LoadCountMax = 0;
            SceneText = txt;

            for (int i = 0; i < line.Length; i++)
            {
                var lineCSV = line[i].Split(',');
                var item = lineCSV[0];
                if (item != "" && item != "Camera" && item != "Light" && item != "PointLight" && item != "VRoidHub")
                {
                    var file = new List<string>();
                    file.Add(item);
                    OnFiles(file, new POINT(0, 0));

                    LoadCountMax++;
                }
                else if (item == "Camera")
                {
                    // カメラは位置と回転を適用してもRTHandlesのPivotに視点が釣られるのでPivotの位置も必要
                    RuntimeSceneComponent.CameraPosition = GetVector3(lineCSV, 1);
                    RuntimeSceneComponent.Pivot = GetVector3(lineCSV, 10);
                }
                else if (item == "Light")
                {
                    Light.transform.position = GetVector3(lineCSV, 1);
                    Light.transform.eulerAngles = GetVector3(lineCSV, 4);
                    Light.GetComponent<LinkLight>().SetDirectionalLight(float.Parse(lineCSV[10]), float.Parse(lineCSV[11]), float.Parse(lineCSV[12]), float.Parse(lineCSV[13]));
                }
                else if (item == "PointLight")
                {
                    var position = GetVector3(lineCSV, 1);
                    var intensity = float.Parse(lineCSV[10]);
                    var color = new Color(float.Parse(lineCSV[11]), float.Parse(lineCSV[12]), float.Parse(lineCSV[13]));
                    var range = float.Parse(lineCSV[14]);
                    Light.GetComponent<LinkLight>().AddPointLight(position, intensity, range, color);
                }
                else if (item == "VRoidHub")
                {
                    // VRoidHubの場合は何もしない（現状諦め）
                }
            }

            return;
        }

        if (!SelectVRM.IsActive)
        {
            return;
        }

        // ポーズテキストは(0,0,0)の場合に値を設定しない（指など一部だけの適用を想定）
        string[] pose = txt.Replace("\n", ",").Split(',');

        for (int i = 0; i < SelectVRM.Marker.List.Count; i++)
        {
            var angle = SelectVRM.Marker.List[i].localEulerAngles;
            if (!SelectVRM.IsFullBone)
            {
                var link = SelectVRM.Marker.List[i].GetComponent<LinkRotation>();
                if (link.IsPrimal)
                {
                    var vec3 = GetVector3(pose, i * 3);
                    angle = vec3 != Vector3.zero ? vec3 : angle;
                    SelectVRM.Marker.List[i].localEulerAngles = angle;
                }
            }
            else
            {
                var vec3 = GetVector3(pose, i * 3);
                angle = vec3 != Vector3.zero ? vec3 : angle;
                SelectVRM.Marker.List[i].localEulerAngles = angle;
            }
        }

        var fingerIndex = txt.IndexOf("Finger") + "Finger\n".Length;
        var fingerTxt = txt.Substring(fingerIndex);
        string[] finger = fingerTxt.Replace("\n", ",").Split(',');

        var start = (int)HumanBodyBones.LeftThumbProximal;
        var end = (int)HumanBodyBones.RightLittleDistal;
        for (var i = 0; i <= end - start; i++)
        {
            var vec3 = GetVector3(finger, i * 3);
            var angle = SelectVRM.Marker.List[i].localEulerAngles;
            var tf = SelectVRM.Animator.GetBoneTransform((HumanBodyBones)i + start);
            tf.localEulerAngles = vec3 != Vector3.zero ? vec3 : angle;
        }
        LinkHand.GetFingerAngle();

        var shapeVRM_Index = txt.IndexOf("BlendShapeVRM") + "BlendShapeVRM\n".Length;
        var shapeVRM_Txt = txt.Substring(shapeVRM_Index);
        string[] shapeVRM = shapeVRM_Txt.Replace("\n", ",").Split(',');

        var clips = SelectVRM.Proxy.BlendShapeAvatar.Clips;
        var values = new Dictionary<BlendShapeKey, float>();
        for (int i = 0; i < clips.Count; i++)
        {
            values.Add(clips[i].Key, float.Parse(shapeVRM[i]));
        }
        SelectVRM.Proxy.SetValues(values);

        var shapeFull_Index = txt.IndexOf("BlendShapeFull") + "BlendShapeFull\n".Length;
        var shapeFull_Txt = txt.Substring(shapeFull_Index);
        string[] shapeFull = shapeFull_Txt.Replace("\n", ",").Split(',');

        var ii = 0;
        var skinneds = SelectVRM.RootMarker.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (var skinned in skinneds)
        {
            for (int i = 0; i < skinned.sharedMesh.blendShapeCount; i++)
            {
                skinned.SetBlendShapeWeight(i, float.Parse(shapeFull[ii]));
                ii++;
            }
        }
        LinkBlendShape.GetBlendShape();

        var IKIndex = txt.IndexOf("IK") + "IK\n".Length;
        var IKTxt = txt.Substring(IKIndex);
        string[] IK = IKTxt.Replace("\n", ",").Split(',');

        var count = SelectVRM.Marker.List.Count;
        SelectVRM.Marker.List[count - 10].localPosition = GetVector3(IK, 0);
        SelectVRM.Marker.List[count - 9].localPosition = GetVector3(IK, 3);
        SelectVRM.Marker.List[count - 8].localPosition = GetVector3(IK, 6);
        SelectVRM.Marker.List[count - 7].localPosition = GetVector3(IK, 9);
        SelectVRM.Marker.List[count - 6].localPosition = GetVector3(IK, 12);
        SelectVRM.Marker.List[count - 5].localPosition = GetVector3(IK, 15);
        SelectVRM.Marker.List[count - 10].localEulerAngles = GetVector3(IK, 18);
        SelectVRM.Marker.List[count - 9].localEulerAngles = GetVector3(IK, 21);
        SelectVRM.Marker.List[count - 8].localEulerAngles = GetVector3(IK, 24);
        SelectVRM.Marker.List[count - 7].localEulerAngles = GetVector3(IK, 27);
        SelectVRM.Marker.List[count - 6].localEulerAngles = GetVector3(IK, 30);
        SelectVRM.Marker.List[count - 5].localEulerAngles = GetVector3(IK, 33);
        SelectVRM.Marker.List[count - 4].localEulerAngles = GetVector3(IK, 36);
        SelectVRM.Marker.List[count - 3].localEulerAngles = GetVector3(IK, 39);
        SelectVRM.Marker.List[count - 2].localEulerAngles = GetVector3(IK, 42);
        SelectVRM.Marker.List[count - 1].localEulerAngles = GetVector3(IK, 45);
    }

    Vector3 GetVector3(string[] pose, int index)
    {
        if (pose.Length <= index + 1) return Vector3.zero; // 最後の改行があるので+1して値有無を確認
        float x = float.Parse(pose[index + 0]);
        float y = float.Parse(pose[index + 1]);
        float z = float.Parse(pose[index + 2]);
        var vec3 = new Vector3(x, y, z);
        return vec3;
    }

    void TriLib(string path)
    {
        var assetLoaderOptions = AssetLoader.CreateDefaultLoaderOptions();
        AssetLoader.LoadModelFromFile(path, null, OnMaterialsLoad, null, null, null, assetLoaderOptions);
        TriLibPath = path;
    }

    string TriLibPath = ""; // ゆるせ、ゆるせ……

    private void OnMaterialsLoad(AssetLoaderContext assetLoaderContext)
    {
        assetLoaderContext.RootGameObject.SetActive(true);

        var markerM = Instantiate(MarkerM).transform;
        markerM.name = "TriLib_Root";
        markerM.position = new Vector3(0, -1, 0);
        markerM.gameObject.AddComponent<SaveSceneTarget>().Path = TriLibPath;
        assetLoaderContext.RootGameObject.transform.position = new Vector3(0, -1, 0);
        assetLoaderContext.RootGameObject.transform.parent = markerM;

        RuntimeSceneComponent.Selection.activeGameObject = markerM.gameObject;

        DandD.SetActive(false);

        LoadCount++;
    }

    public void Shuffle()
    {
        // 前の背景があれば削除
        if (Background != null)
        {
            Destroy(Background);
        }

        // 背景をランダム設定
        string[] bgs_jpg = Directory.GetFiles(@"Background\", "*.jpg");
        string[] bgs_png = Directory.GetFiles(@"Background\", "*.png");
        string[] bgs = new string[bgs_jpg.Length + bgs_png.Length];
        Array.Copy(bgs_jpg, bgs, bgs_jpg.Length);
        Array.Copy(bgs_png, 0, bgs, bgs_jpg.Length, bgs_png.Length);
        Background = IMG(bgs[UnityEngine.Random.Range(0, bgs.Length)]);

        // ポーズをランダム設定
        string[] poses = Directory.GetFiles(@"Pose\", "*.png");
        IMG(poses[UnityEngine.Random.Range(0, poses.Length)]);
    }

    void SetRagDoll()
    {
        if (SelectVRM.Animator == null) return;

        // ラグドール設定
        var anim = SelectVRM.Animator;
        var r = new BipedRagdollReferences();
        r.root = SelectVRM.Animator.gameObject.transform;
        r.hips = anim.GetBoneTransform(HumanBodyBones.Hips);
        r.spine = anim.GetBoneTransform(HumanBodyBones.Spine);
        r.chest = anim.GetBoneTransform(HumanBodyBones.Chest);
        r.head = anim.GetBoneTransform(HumanBodyBones.Head);
        r.leftUpperLeg = anim.GetBoneTransform(HumanBodyBones.LeftUpperLeg);
        r.leftLowerLeg = anim.GetBoneTransform(HumanBodyBones.LeftLowerLeg);
        r.leftFoot = anim.GetBoneTransform(HumanBodyBones.LeftFoot);
        r.rightUpperLeg = anim.GetBoneTransform(HumanBodyBones.RightUpperLeg);
        r.rightLowerLeg = anim.GetBoneTransform(HumanBodyBones.RightLowerLeg);
        r.rightFoot = anim.GetBoneTransform(HumanBodyBones.RightFoot);
        r.leftUpperArm = anim.GetBoneTransform(HumanBodyBones.LeftUpperArm);
        r.leftLowerArm = anim.GetBoneTransform(HumanBodyBones.LeftLowerArm);
        r.leftHand = anim.GetBoneTransform(HumanBodyBones.LeftHand);
        r.rightUpperArm = anim.GetBoneTransform(HumanBodyBones.RightUpperArm);
        r.rightLowerArm = anim.GetBoneTransform(HumanBodyBones.RightLowerArm);
        r.rightHand = anim.GetBoneTransform(HumanBodyBones.RightHand);

        BipedRagdollCreator.Create(r, BipedRagdollCreator.Options.Default);

        // ポーズ操作用マーカーの無効化
        foreach (var m in SelectVRM.Marker.List)
        {
            m.gameObject.SetActive(false);
        }
    }

    void DisableGravity()
    {
        if (SelectVRM.Animator == null) return;

        // ラグドール設定向けの重力無効化
        var rigids = SelectVRM.Animator.gameObject.GetComponentsInChildren<Rigidbody>();
        foreach (var rigid in rigids)
        {
            rigid.useGravity = false;
        }
    }

    void DestroyJointAndRigid()
    {
        if (SelectVRM.Animator == null) return;

        // ラグドール解除（再設定向け）
        var joints = SelectVRM.Animator.gameObject.GetComponentsInChildren<ConfigurableJoint>();
        foreach (var joint in joints)
        {
            Destroy(joint);
        }
        var rigids = SelectVRM.Animator.gameObject.GetComponentsInChildren<Rigidbody>();
        foreach (var rigid in rigids)
        {
            rigid.useGravity = false;
            if (!rigid.isKinematic)
            {
                Destroy(rigid);
            }
        }
    }
}
