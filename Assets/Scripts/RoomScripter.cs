using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Moods = LightingHelper.Moods;

public class RoomScripter : MonoBehaviour
{
    private RoomManager roomManager;
    public Textures textures;
    public Sounds sounds;

    // Start is called before the first frame update
    void Start()
    {
        roomManager = FindObjectOfType<RoomManager>();

        // Room 403
        Room Room403 = roomManager.GetRoomByName("403");
        Room403.SetAllWallsTexture(textures.brick);
        Room403.SetFloorTexture(textures.grass);
        Room403.SetCeilingTexture(textures.water);
        Room403.SetSoundEffect(sounds.talkingStudents);
        Room403.SpawnSpheres("red", 5);
        Room403.SetMood(Moods.Misty);


        // Room 405
        Room Room405 = roomManager.GetRoomByName("405");
        Room405.SetAllWallsTexture(textures.brick);
        Room405.SetFloorTexture(textures.grass);
        Room405.SetCeilingTexture(textures.water);
        Mood newMood = LightingHelper.Instance.CreateMood(Color.red, 1f, Color.magenta, 0.5f, 5f, Color.yellow,
            new Color(0.5f, 0.5f, 0.5f, 0.1f), null, 0);
        Room405.SetMood(newMood);

        // Room 406
        Room Room406 = roomManager.GetRoomByName("406");
        Room406.SetAllWallsTexture(textures.brick);
        Room406.SetFloorTexture(textures.grass);
        Room406.SetCeilingTexture(textures.water);


        // Room 407
        Room Room407 = roomManager.GetRoomByName("407");
        Room407.SetAllWallsTexture(textures.brick);
        Room407.SetFloorTexture(textures.grass);
        Room407.SetCeilingTexture(textures.water);
        Room407.SetSoundEffect(sounds.talkingStudents);
        Room407.SpawnSpheres("blue", 3);


        // Room 408
        Room Room408 = roomManager.GetRoomByName("408");
        Room408.SetAllWallsTexture(textures.brick);
        Room408.SetFloorTexture(textures.grass);
        Room408.SetCeilingTexture(textures.water);


        // Room 409
        Room Room409 = roomManager.GetRoomByName("409");
        Room409.SetAllWallsTexture(textures.brick);
        Room409.SetFloorTexture(textures.grass);
        Room409.SetCeilingTexture(textures.water);


        // Room 410A
        Room Room410A = roomManager.GetRoomByName("410A");
        Room410A.SetAllWallsTexture(textures.cobblestone);
        Room410A.SetFloorTexture(textures.ice);
        Room410A.SetCeilingTexture(textures.water);


        // Room 410B
        Room Room410B = roomManager.GetRoomByName("410B");
        Room410B.SetAllWallsTexture(textures.cobblestone);
        Room410B.SetFloorTexture(textures.ice);
        Room410B.SetCeilingTexture(textures.water);


        // Room 410C
        Room Room410C = roomManager.GetRoomByName("410C");
        Room410C.SetAllWallsTexture(textures.cobblestone);
        Room410C.SetFloorTexture(textures.ice);
        Room410C.SetCeilingTexture(textures.water);


        // Room 411
        Room Room411 = roomManager.GetRoomByName("411");
        Room411.SetAllWallsTexture(textures.brick);
        Room411.SetFloorTexture(textures.grass);
        Room411.SetCeilingTexture(textures.water);


        // Room 412
        Room Room412 = roomManager.GetRoomByName("412");
        Room412.SetAllWallsTexture(textures.brick);
        Room412.SetFloorTexture(textures.grass);
        Room412.SetCeilingTexture(textures.water);


        // Room 414
        Room Room414 = roomManager.GetRoomByName("414");
        Room414.SetAllWallsTexture(textures.brick);
        Room414.SetFloorTexture(textures.grass);
        Room414.SetCeilingTexture(textures.water);


        // Room 415
        Room Room415 = roomManager.GetRoomByName("415");
        Room415.SetAllWallsTexture(textures.wood);
        Room415.SetFloorTexture(textures.lava);
        Room415.SetCeilingTexture(textures.sand);
        Room415.SetSoundEffect(sounds.talkingStudents);
        Room415.SpawnSpheres("green", 5);


        // Room 417
        Room Room417 = roomManager.GetRoomByName("417");
        Room417.SetAllWallsTexture(textures.wood);
        Room417.SetFloorTexture(textures.lava);
        Room417.SetCeilingTexture(textures.sand);


        // Room 425
        Room Room425 = roomManager.GetRoomByName("425");
        Room425.SetAllWallsTexture(textures.water);
        Room425.SetFloorTexture(textures.water);
        Room425.SetCeilingTexture(textures.water);
        Room425.SetSoundEffect(sounds.talkingStudents);
        Room425.SpawnSpheres("green", 30);


        // Room 427
        Room Room427 = roomManager.GetRoomByName("427");
        Room427.SetAllWallsTexture(textures.wood);
        Room427.SetFloorTexture(textures.lava);
        Room427.SetCeilingTexture(textures.sand);
        Room427.SetMood(Moods.Default);


        // Room 430
        Room Room430 = roomManager.GetRoomByName("430");
        Room430.SetAllWallsTexture(textures.wood);
        Room430.SetFloorTexture(textures.lava);
        Room430.SetCeilingTexture(textures.sand);


        // Room 431
        Room Room431 = roomManager.GetRoomByName("431");
        Room431.SetAllWallsTexture(textures.wood);
        Room431.SetFloorTexture(textures.lava);
        Room431.SetCeilingTexture(textures.sand);
        Room431.SetMood(Moods.Misty);


        // Room 432
        Room Room432 = roomManager.GetRoomByName("432");
        Room432.SetAllWallsTexture(textures.wood);
        Room432.SetFloorTexture(textures.lava);
        Room432.SetCeilingTexture(textures.sand);


        // Room 433
        Room Room433 = roomManager.GetRoomByName("433");
        Room433.SetAllWallsTexture(textures.wood);
        Room433.SetFloorTexture(textures.lava);
        Room433.SetCeilingTexture(textures.sand);
        Room433.SetMood(Moods.Emblazen);


        // Room 434
        Room Room434 = roomManager.GetRoomByName("434");
        Room434.SetAllWallsTexture(textures.wood);
        Room434.SetFloorTexture(textures.lava);
        Room434.SetCeilingTexture(textures.sand);
        Room434.SetMood(Moods.Flourescent);


        // Room 435
        Room Room435 = roomManager.GetRoomByName("435");
        Room435.SetAllWallsTexture(textures.wood);
        Room435.SetFloorTexture(textures.lava);
        Room435.SetCeilingTexture(textures.sand);
        Room435.SetMood(Moods.Darkness);


        // Room 436
        Room Room436 = roomManager.GetRoomByName("436");
        Room436.SetAllWallsTexture(textures.wood);
        Room436.SetFloorTexture(textures.lava);
        Room436.SetCeilingTexture(textures.sand);


        // Room 439
        Room Room439 = roomManager.GetRoomByName("439");
        Room439.SetAllWallsTexture(textures.wood);
        Room439.SetFloorTexture(textures.lava);
        Room439.SetCeilingTexture(textures.sand);
        Room439.SetMood(Moods.Cozy);


        // Room 441
        Room Room441 = roomManager.GetRoomByName("441");
        Room441.SetAllWallsTexture(textures.water);
        Room441.SetFloorTexture(textures.cobblestone);
        Room441.SetCeilingTexture(textures.sand);


        // Room 443
        Room Room443 = roomManager.GetRoomByName("443");
        Room443.SetAllWallsTexture(textures.water);
        Room443.SetFloorTexture(textures.cobblestone);
        Room443.SetCeilingTexture(textures.sand);

        // Room 444
        Room Room444 = roomManager.GetRoomByName("444");
        Room444.SetAllWallsTexture(textures.bones);
        Room444.SetFloorTexture(textures.cobblestone);
        Room444.SetCeilingTexture(textures.forest);
        Room415.SetSoundEffect(sounds.talkingStudents);
        Room415.SpawnSpheres("red", 2);


        // Room 448
        Room Room448 = roomManager.GetRoomByName("448");
        Room448.SetAllWallsTexture(textures.bones);
        Room448.SetFloorTexture(textures.cobblestone);
        Room448.SetCeilingTexture(textures.forest);


        // Room 449
        Room Room449 = roomManager.GetRoomByName("449");
        Room449.SetAllWallsTexture(textures.bones);
        Room449.SetFloorTexture(textures.cobblestone);
        Room449.SetCeilingTexture(textures.forest);


        // Room 452
        Room Room452 = roomManager.GetRoomByName("452");
        Room452.SetAllWallsTexture(textures.bones);
        Room452.SetFloorTexture(textures.cobblestone);
        Room452.SetCeilingTexture(textures.forest);


        // Room 455
        Room Room455 = roomManager.GetRoomByName("455");
        Room455.SetAllWallsTexture(textures.bones);
        Room455.SetFloorTexture(textures.cobblestone);
        Room455.SetCeilingTexture(textures.forest);


        // Room 456
        Room Room456 = roomManager.GetRoomByName("456");
        Room456.SetAllWallsTexture(textures.bones);
        Room456.SetFloorTexture(textures.cobblestone);
        Room456.SetCeilingTexture(textures.forest);


        // Room 457
        Room Room457 = roomManager.GetRoomByName("457");
        Room457.SetAllWallsTexture(textures.bones);
        Room457.SetFloorTexture(textures.cobblestone);
        Room457.SetCeilingTexture(textures.forest);


        // Room 459
        Room Room459 = roomManager.GetRoomByName("459");
        Room459.SetAllWallsTexture(textures.bones);
        Room459.SetFloorTexture(textures.cobblestone);
        Room459.SetCeilingTexture(textures.forest);


        // Principle's Office
        Room PO = roomManager.GetRoomByName("PO");
        PO.SetAllWallsTexture(textures.bones);
        PO.SetFloorTexture(textures.sand);
        PO.SetCeilingTexture(textures.ice);


        // Thrift Closet
        Room Thrift = roomManager.GetRoomByName("Thrift");
        Thrift.SetAllWallsTexture(textures.bones);
        Thrift.SetFloorTexture(textures.sand);
        Thrift.SetCeilingTexture(textures.ice);
        Thrift.SetSoundEffect(sounds.talkingStudents);
        Thrift.SpawnSpheres("green", 2);
        
        //update fog
        LightingHelper.Instance.UpdateFog();
    }

}