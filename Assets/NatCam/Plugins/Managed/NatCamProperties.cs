﻿/* 
*   NatCam
*   Copyright (c) 2016 Yusuf Olokoba
*/


using UnityEngine;
using System;
using System.Collections.Generic;
using NatCamU.Internals;

namespace NatCamU {
    
    //Delegates
    
    ///<summary>
    ///A delegate type that NatCam uses to pass a captured photo to subscribers.
    ///</summary>
	[NCDoc(24)] public delegate void PhotoCallback (Texture2D photo);
    ///<summary>
    ///A delegate type that NatCam uses to pass information about detected barcodes to subscribers.
    ///</summary>
	[NCDoc(23)] public delegate void BarcodeCallback (List<Barcode> codes);
    ///<summary>
    ///A delegate type that NatCam uses to initialize NatCamPreviewBehaviours.
    ///</summary>
	[NCDoc(22)] public delegate void PreviewCallback ();
    ///<summary>
    ///A delegate type that NatCam uses to provide the native pointer to the preview pixel data when it is available.
    ///</summary>
	[NCDoc(21)] public delegate void NativePreviewCallback (ComponentBuffer bufferType, IntPtr nativePointer, int width, int height, int size);

	//Enumerations
	[NCDoc(90)] public enum Facing {
		[NCDoc(91)] Rear = 0,
		[NCDoc(92)] Front = 1
    }
	[NCDoc(25)] public enum FrameratePreset : byte {
		[NCDoc(104)] Default = 30,
        [NCDoc(105)] Smooth = 60,
        [NCDoc(106)] SlowMotion = 120,
        [NCDoc(107)] HighestFramerate = 240,
        [NCDoc(108)] LowestFramerate = 15
    }
	[NCDoc(93)] public enum NatCamInterface : byte {
		[NCDoc(94)] NativeInterface = 0,
		[NCDoc(95)] FallbackInterface = 1
    }
	[NCDoc(9)] public enum ResolutionPreset : byte {
		[NCDoc(26)] HD = 0,
		[NCDoc(27)] FullHD = 1,
		[NCDoc(28)] HighestResolution = 2,
        [NCDoc(29)] MediumResolution = 3,
		[NCDoc(30)] LowestResolution = 4,
	}
	[NCDoc(96)] public enum ComponentBuffer : byte {
		[NCDoc(97)] RGBA32 = 0,
        [NCDoc(4, 98)] Y4 = 1,
        [NCDoc(5, 99)] UV2 = 2,
        [NCDoc(100)] RGBA32GPU = 3,
        [NCDoc(101)] Y4GPU = 4,
    }
	[NCDoc(11)] public enum FocusMode : byte {
        [NCDoc(63)] AutoFocus = 0,
        [NCDoc(64)] TapToFocus = 1,
        [NCDoc(2, 65)] HybridFocus = 2,
        [NCDoc(66)] Off = 3
    }
	[NCDoc(10)] public enum FlashMode : byte {
		[NCDoc(31)] Auto = 0,
		[NCDoc(32)] On = 1,
		[NCDoc(33)] Off = 2
	}
	[NCDoc(12)] [Flags] public enum BarcodeFormat : byte { //Update native mappings
		[NCDoc(34)] QR = 1,
		[NCDoc(35)] EAN_13 = 2,
		[NCDoc(36)] EAN_8 = 4,
		[NCDoc(37)] DATA_MATRIX = 8,
		[NCDoc(38)] PDF_417 = 16,
		[NCDoc(39)] CODE_128 = 32,
		[NCDoc(40)] CODE_93 = 64,
		[NCDoc(41)] CODE_39 = 128,
		[NCDoc(42)] ALL = 255
	}
	[NCDoc(13)] public enum Switch : byte {
		[NCDoc(43)] Off = 0,
		[NCDoc(44)] On = 1
	}
	[NCDoc(14)] public enum PhotoSaveMode : byte {
		[NCDoc(45)] DoNotSave = 0,
		[NCDoc(46)] SaveToPhotos = 1,
		[NCDoc(47)] SaveToAppAlbum = 2
	}
	[NCDoc(15)] public enum PreviewType : byte {
		[NCDoc(48)] NonReadable = 0,
		[NCDoc(3, 49)] Readable = 1
	}
	[NCDoc(16)] public enum ScaleMode : byte {
		[NCDoc(50)] FixedWidthVariableHeight = 0,
		[NCDoc(51)] FixedHeightVariableWidth = 1,
		[NCDoc(52)] FillScreen = 2,
		[NCDoc(53)] None = 3
	}
	[NCDoc(17)] public enum ZoomMode : byte {
		[NCDoc(54)] DigitalZoomAsFallback = 0,
		[NCDoc(55)] ForceDigitalZoomOnly = 1,
		[NCDoc(56)] ZoomSpeedOverrideOnly = 2
	}

	//Structs
    [NCDoc(18)]
	public struct Barcode {
		[NCDoc(57)] public readonly string data;
		[NCDoc(58)] public readonly BarcodeFormat format;
		[NCDoc(59)] public Barcode (string Data, BarcodeFormat Format) {
			data = Data;
			format = Format;
		}
		[NCDoc(60)] [NCCode(1)] public override string ToString () {
			return "["+format+"]:"+data;
		}
	}
    
	[NCDoc(85)]
    public struct BarcodeRequest {
		[NCDoc(86)] [NCRef(6)] public readonly BarcodeCallback callback;
		[NCDoc(87)] [NCRef(7)] public readonly BarcodeFormat format;
		[NCDoc(88)] public readonly bool unsubscribeOnceDetected;
		[NCDoc(89)] public BarcodeRequest (BarcodeCallback Callback, BarcodeFormat preferredFormat = BarcodeFormat.ALL, bool automaticallyUnsubscribe = true) {
            callback = Callback;
            format = preferredFormat;
            unsubscribeOnceDetected = automaticallyUnsubscribe;
        }
    }
}