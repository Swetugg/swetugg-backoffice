-- clean out all the schedule data for stockholm 2023
-- room, slot, roomslot
-- update tabels
ALTER TABLE Rooms
ADD SessionizeId int;
