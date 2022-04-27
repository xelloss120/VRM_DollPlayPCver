﻿using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
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
        VRM(@"D:\play\UnityProject\VRM_DollPlay2018\_exe\VRM\AoUni.vrm");
        //VRM(@"D:\play\UnityProject\VRM_DollPlay2018\_exe\VRM\old\AliciaSolid.vrm");
        //VRM(@"D:\play\UnityProject\VRM_DollPlay2018\_exe\VRM\AoUniJacket.vrm");
        //IMG(@"D:\play\UnityProject\VRM_DollPlayPCprot\Photo\20191019072142.jpg");
        //GLB(@"D:\play\UnityProject\VRM_DollPlay2018\_exe\GLB\Cube.glb");
        //VRM(@"D:\play\UnityProject\VRM_DollPlay2018\_exe\VRM\Oroka2.vrm");
        //VRM(@"D:\play\UnityProject\VRM_DollPlay2018\_exe\VRM\AoLinz.vrm");
        //TriLib(@"D:\play\Model\Reeva_v1.0.5\Models\Model_scooter.fbx");
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
        GLB(@"D:\play\UnityProject\VRM_DollPlayPCver\_exe\ExampleBackground.glb");
    }

    public void AB_TEST()
    {
        AB(@"D:\play\UnityProject\VRM_DollPlay2018\_exe\ANI\crs_full.ab");
    }

    public void TXT_TEST()
    {
        //TXT(@"D:\play\UnityProject\VRM_DollPlayPCver\TEST\pose_humn.txt");
        //TXT(@"D:\play\UnityProject\VRM_DollPlayPCver\TEST\pose_full.txt");
        TXT(@"C:\Users\xello\Desktop\pose.txt");
    }

    public void PNG_TEST()
    {
        //IMG(@"D:\play\UnityProject\VRM_DollPlayPCver\_exe\pose.png");
        //IMG(@"D:\play\UnityProject\VRM_DollPlayPCver\TEST\pose.png");
        IMG(@"C:\Users\xello\Desktop\pose.png");
    }

    public void TriLib_TEST()
    {
        TriLib(@"D:\play\Model\Reeva_v1.0.5\Models\Model_scooter.fbx");
    }

    void VRM(string path)
    {
        var data = new AutoGltfFileParser(path).Parse();
        var vrm = new VRMData(data);
        IMaterialDescriptorGenerator materialGen = default;
        using (var loader = new VRMImporterContext(vrm, materialGenerator: materialGen))
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
        IMaterialDescriptorGenerator materialGen = default;
        using (var loader = new ImporterContext(data, materialGenerator: materialGen))
        {
            var markerM = Instantiate(MarkerM).transform;
            markerM.name = "GLB_Root";
            markerM.position = new Vector3(0, 0, 0);
            markerM.gameObject.AddComponent<SaveSceneTarget>().Path = path;

            var instance = loader.Load();
            instance.Root.transform.parent = markerM;
            instance.ShowMeshes();

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
                    var values = new Dictionary<BlendShapeKey, float>();
                    values.Add(BlendShapeKey.CreateFromPreset(BlendShapePreset.Neutral), ColorToFloat(color[index + 0]));
                    values.Add(BlendShapeKey.CreateFromPreset(BlendShapePreset.A), ColorToFloat(color[index + 1]));
                    values.Add(BlendShapeKey.CreateFromPreset(BlendShapePreset.I), ColorToFloat(color[index + 2]));
                    values.Add(BlendShapeKey.CreateFromPreset(BlendShapePreset.U), ColorToFloat(color[index + 3]));
                    values.Add(BlendShapeKey.CreateFromPreset(BlendShapePreset.E), ColorToFloat(color[index + 4]));
                    values.Add(BlendShapeKey.CreateFromPreset(BlendShapePreset.O), ColorToFloat(color[index + 5]));
                    values.Add(BlendShapeKey.CreateFromPreset(BlendShapePreset.Blink), ColorToFloat(color[index + 6]));
                    values.Add(BlendShapeKey.CreateFromPreset(BlendShapePreset.Joy), ColorToFloat(color[index + 7]));
                    values.Add(BlendShapeKey.CreateFromPreset(BlendShapePreset.Angry), ColorToFloat(color[index + 8]));
                    values.Add(BlendShapeKey.CreateFromPreset(BlendShapePreset.Sorrow), ColorToFloat(color[index + 9]));
                    values.Add(BlendShapeKey.CreateFromPreset(BlendShapePreset.Fun), ColorToFloat(color[index + 10]));
                    values.Add(BlendShapeKey.CreateFromPreset(BlendShapePreset.LookUp), ColorToFloat(color[index + 11]));
                    values.Add(BlendShapeKey.CreateFromPreset(BlendShapePreset.LookDown), ColorToFloat(color[index + 12]));
                    values.Add(BlendShapeKey.CreateFromPreset(BlendShapePreset.LookLeft), ColorToFloat(color[index + 13]));
                    values.Add(BlendShapeKey.CreateFromPreset(BlendShapePreset.LookRight), ColorToFloat(color[index + 14]));
                    values.Add(BlendShapeKey.CreateFromPreset(BlendShapePreset.Blink_L), ColorToFloat(color[index + 15]));
                    values.Add(BlendShapeKey.CreateFromPreset(BlendShapePreset.Blink_R), ColorToFloat(color[index + 16]));
                    SelectVRM.Proxy.SetValues(values);
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

        if (csv.Length == 10)
        {
            // シーン読込
            LoadCount = 0;
            LoadCountMax = 0;
            SceneText = txt;

            for (int i = 0; i < line.Length; i++)
            {
                var lineCSV = line[i].Split(',');
                var item = lineCSV[0];
                if (item != "" && item != "Camera" && item != "Light")
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
                }
            }

            return;
        }

        if (!SelectVRM.IsActive)
        {
            return;
        }

        string[] pose = txt.Replace("\n", ",").Split(',');

        for (int i = 0; i < SelectVRM.Marker.List.Count; i++)
        {
            // (0,0,0)は角度を設定しないように（指など一部だけの適用を想定）
            var vec3 = GetVector3(pose, i * 3);
            var angle = SelectVRM.Marker.List[i].localEulerAngles;
            angle = vec3 != Vector3.zero ? vec3 : angle;
            SelectVRM.Marker.List[i].localEulerAngles = angle;
        }
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
        markerM.position = new Vector3(0, 0, 0);
        markerM.gameObject.AddComponent<SaveSceneTarget>().Path = TriLibPath;
        assetLoaderContext.RootGameObject.transform.parent = markerM;

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
