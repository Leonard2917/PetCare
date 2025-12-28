USE PetCare;
GO

-- Create ProgramariServicii junction table for Many-to-Many relationship
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ProgramariServicii')
BEGIN
    CREATE TABLE ProgramariServicii (
        ProgramareID int NOT NULL,
        ServiciuID int NOT NULL,
        PRIMARY KEY (ProgramareID, ServiciuID),
        CONSTRAINT FK_ProgramariServicii_Programari FOREIGN KEY (ProgramareID) REFERENCES Programari(ProgramareID) ON DELETE CASCADE,
        CONSTRAINT FK_ProgramariServicii_Servicii FOREIGN KEY (ServiciuID) REFERENCES Servicii(ServiciuID)
    );
    PRINT 'Table ProgramariServicii created successfully.';
END
ELSE
BEGIN
    PRINT 'Table ProgramariServicii already exists.';
END
GO
