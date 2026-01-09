USE PetCare;
GO

-- 1. Create MediciClinici junction table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'MediciClinici')
BEGIN
    CREATE TABLE MediciClinici (
        MedicID int NOT NULL,
        ClinicaID int NOT NULL,
        PRIMARY KEY (MedicID, ClinicaID),
        CONSTRAINT FK_MediciClinici_Medici FOREIGN KEY (MedicID) REFERENCES Medici(MedicID),
        CONSTRAINT FK_MediciClinici_Clinici FOREIGN KEY (ClinicaID) REFERENCES Clinici(ClinicaID)
    );
    PRINT 'Table MediciClinici created.';
END

-- 2. Migrate existing relationships
-- Only insert if not exists to avoid duplicates if re-run
INSERT INTO MediciClinici (MedicID, ClinicaID)
SELECT MedicID, ClinicaID
FROM Medici
WHERE ClinicaID IS NOT NULL
AND NOT EXISTS (
    SELECT 1 FROM MediciClinici mc 
    WHERE mc.MedicID = Medici.MedicID AND mc.ClinicaID = Medici.ClinicaID
);
PRINT 'Existing Medic-Clinic data migrated.';

-- 3. Update OrarMedici
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('OrarMedici') AND name = 'ClinicaID')
BEGIN
    ALTER TABLE OrarMedici ADD ClinicaID int;
    PRINT 'Column ClinicaID added to OrarMedici.';
END
GO

-- Backfill ClinicaID based on current Medic's clinic
UPDATE OrarMedici 
SET ClinicaID = m.ClinicaID 
FROM OrarMedici o 
JOIN Medici m ON o.MedicID = m.MedicID
WHERE o.ClinicaID IS NULL;
PRINT 'OrarMedici backfilled.';

-- Add FK
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_OrarMedici_Clinici')
BEGIN
    ALTER TABLE OrarMedici ADD CONSTRAINT FK_OrarMedici_Clinici FOREIGN KEY (ClinicaID) REFERENCES Clinici(ClinicaID);
    PRINT 'FK_OrarMedici_Clinici created.';
END
GO
