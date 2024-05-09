
#region Defining Variables and Arrays

Item keyToMaster = new Item { name = "Key to Master Room", isInInv = false };

Item keyToFreedom = new Item { name = "Key to Freedom", isInInv = false };

Room[] Rooms = new Room[9];

Rooms[0] = new Room
{
    identifier = 0,
    name = "General Space",
    roomDesc = "Main Enterence and A corridor to access all rooms",
    isLocked = false
};
Rooms[1] = new Room
{
    identifier = 1,
    name = "Storage",
    roomDesc = "Storage room and Laundy machine",
    isLocked = false
};
Rooms[2] = new Room
{
    identifier = 2,
    name = "Living Room",
    roomDesc = "Living room includes TV, Sofa, Clock, coffee table etc..",
    isLocked = false
};
Rooms[3] = new Room
{
    identifier = 3,
    name = "Kitchen",
    roomDesc = "Kithcen includes Induction, Mixer, Dining Table, Fridge, Utensils, Dishwasher etc..",
    isLocked = false
};
Rooms[4] = new Room
{
    identifier = 4,
    name = "Washroom1",
    roomDesc = "Washroom 1 with Washbasin.",
    isLocked = false
};
Rooms[5] = new Room
{
    identifier = 5,
    name = "Washroom2",
    roomDesc = "Washroom 2 with Washbasin.",
    isLocked = false
};
Rooms[6] = new Room
{
    identifier = 6,
    name = "Master Room",
    roomDesc = "Master Bedroom with huge space, balcony, Coffee table, sofa, AC, Heater, Lamp, TV, etc...",
    isLocked = true,
    items = new Item[] { keyToFreedom },
    specialCondition = "Key to Master Room"
};
Rooms[7] = new Room
{
    identifier = 7,
    name = "Bedroom",
    roomDesc = "Bedroom with small space, AC, Heater, Lamp",
    isLocked = false,
    items = new Item[] { keyToMaster }
};
Rooms[8] = new Room
{
    identifier = 8,
    name = "Outside",
    roomDesc = "The frontside of the house",
    isLocked = true,
    specialCondition = "Key to Freedom"
};

Rooms[0].connections = new Room[] { Rooms[1], Rooms[2], Rooms[3], Rooms[4], Rooms[5], Rooms[6], Rooms[7], Rooms[8] };
Rooms[1].connections = new Room[] { Rooms[0] };
Rooms[2].connections = new Room[] { Rooms[0] };
Rooms[3].connections = new Room[] { Rooms[0] };
Rooms[4].connections = new Room[] { Rooms[0] };
Rooms[5].connections = new Room[] { Rooms[0] };
Rooms[6].connections = new Room[] { Rooms[0] };
Rooms[7].connections = new Room[] { Rooms[0] };
Rooms[8].connections = new Room[] { Rooms[0] };

Player player = new Player { position = Rooms[0], inventory = new Item[3] };

#endregion

#region Main Loop

int rounds = 0;
WriteGameinfo(player, start: true);
while (true) {
    Console.Clear();
    bool playerChooseRoom = false;
    bool playerChooseItem = false;

    WriteGameinfo(player);
    WriteRoomInfo(player.position);
    WriteRoomOptions(player.position);

    int finalInput = UserFinalInput(ref player, ref playerChooseRoom, ref playerChooseItem);
    if (playerChooseRoom) PlayerChooseRoom(player, finalInput);
    if (playerChooseItem) PlayerChooseItem(player, finalInput);

    if (player.position.specialCondition == "Key to Freedom") break;
    rounds++;
}

for (int i = 0; i < 9; i++) {
    ChangeColor(i);
    Console.WriteLine("Congratulations! You escaped the house.");
    Thread.Sleep(500); // Makes the program waits for half a second
}

#endregion

#region Functions

void PlayerChooseItem(Player player, int finalInput)
{
    int i = 0;
    foreach (Item item in player.inventory)
    {
        if (item is null)
        {
            player.inventory[i] = player.position.items[finalInput - 1];
            player.position.items[finalInput - 1].isInInv = true;
            break;
        }
        i++;
    }
}

void PlayerChooseRoom(Player player, int finalInput)
{
    if (player.position.connections[finalInput - 1].isLocked)
    {
        Console.WriteLine("This door is locked!");
        foreach (Item item in player.inventory)
        {
            if (item is not null && item.name == player.position.connections[finalInput - 1].specialCondition)
            {
                player.position.connections[finalInput - 1].isLocked = false;
                Console.WriteLine($"you used {item.name} to unlock it!");
                player.position = player.position.connections[finalInput - 1];
            }
        }
        Console.WriteLine("Press ENTER to continue");
        Console.ReadLine();
    }
    else
    {
        player.position = player.position.connections[finalInput - 1];
    }
}

int UserFinalInput(ref Player player, ref bool playerChooseRoom, ref bool playerChooseItem)
{
    int intUserInput;
    Console.Write($"Select a room to enter \n >>> ");
    string userInput = (Console.ReadLine()!);

    int optionsCounter = player.position.connections.Length;
    if (player.position.items is not null)
    {
        foreach (Item item in player.position.items)
        {
            if (item is not null && !item.isInInv) optionsCounter++;
        }
    }

    while (!TryValidateInput(userInput, out intUserInput, optionsCounter))
    {
        userInput = (Console.ReadLine()!);
    }

    if (intUserInput <= player.position.connections.Length)
    {
        playerChooseRoom = true; return intUserInput;
    }
    else
    {
        playerChooseItem = true; return intUserInput - player.position.connections.Length;
    }
}

void WriteGameinfo(Player player, bool start = false)
{
    if (start)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(" ==========================");
        Console.WriteLine(" =====- A.R.D Escape -=====");
        Console.WriteLine(" ==========================");
        Console.Write("Set a name for your character: \n >>> ");
        player.identifier = Console.ReadLine()!;
        Console.ResetColor();
    }
    else
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(" =====- A.R.D Escape -=====");
        Console.WriteLine($"{player.identifier}'s Inventory: ");
        foreach (Item item in player.inventory)
        {
            if (item is not null) { Console.WriteLine($"[ {item.name} ] "); }
        }
        Console.WriteLine();
        Console.ResetColor();
    }

}

bool TryValidateInput(string userInput, out int intUserInput, int length)
{
    if (int.TryParse(userInput, out int integer))
    {
        if (integer > 0 && integer <= length)
        {
            intUserInput = integer;
            return true;
        }
        else
        {
            Console.WriteLine($"I dont know this room.");
        }
    }
    else
    {
        Console.WriteLine($"Invalid input. Choose a integer between 1 and {length}");
    }
    intUserInput = 0;
    return false;
}

void WriteRoomInfo(Room room)
{
    if (room.items is null)
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"-= {room.name} =-");
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine($"{room.roomDesc}\n Connected Rooms:\n");
    Console.ResetColor();
}

void WriteRoomOptions(Room room)
{
    int i = 1;
    Console.ForegroundColor = ConsoleColor.Green;
    foreach (Room connectedRoom in room.connections) 
    { 
        Console.WriteLine($"[ {i} ]: {connectedRoom.name}");
        i++;
    }
    Console.ForegroundColor = ConsoleColor.Blue;
    if (room.items is not null)
    {
        foreach (Item item in room.items)
        {
            if (!item.isInInv) Console.WriteLine($"[ {i} ]: {item.name}");
        }
        Console.ResetColor();
    }
}

void ChangeColor(int index)
{
    Console.Clear();
    switch (index) {
        case 0: Console.ForegroundColor = ConsoleColor.Cyan; break;
        case 1: Console.ForegroundColor = ConsoleColor.Cyan; break;
        case 2: Console.ForegroundColor = ConsoleColor.DarkCyan; break;
        case 3: Console.ForegroundColor = ConsoleColor.DarkBlue; break;
        case 4: Console.ForegroundColor = ConsoleColor.Blue; break;
        case 5: Console.ForegroundColor = ConsoleColor.Yellow; break;
        case 6: Console.ForegroundColor = ConsoleColor.DarkYellow; break;
        case 7: Console.ForegroundColor = ConsoleColor.DarkGreen; break;
        case 8: Console.ForegroundColor = ConsoleColor.Green; break;
        default: Console.ForegroundColor = ConsoleColor.Cyan; break;
    }

}
#endregion

#region Classes

class Room
{
    public int identifier;
    public string name;
    public string roomDesc;
    public Room[] connections;
    public Item[] items;
    public bool isLocked;
    public string specialCondition;
}

class Player
{
    public string identifier;
    public Room position;
    public Item[]? inventory;
}

class Item
{
    public string name;
    public bool isInInv;
}

#endregion

