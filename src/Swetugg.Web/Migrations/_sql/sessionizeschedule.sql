-- clean out all the schedule data for stockholm 2023
-- room, slot, roomslot
-- update tabels
ALTER TABLE Room
ADD SessionizeId int;
