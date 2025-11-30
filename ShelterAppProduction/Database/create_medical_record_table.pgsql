CREATE TABLE IF NOT EXISTS MedicalRecord (
    Id SERIAL PRIMARY KEY,
    IdAnimal INTEGER NOT NULL,
    IdVeterinarian INTEGER NOT NULL,
    VisitDate DATE NOT NULL,
    Diagnosis TEXT,
    Treatment TEXT,
    Notes TEXT,
    CONSTRAINT FK_MedicalRecord_Animal FOREIGN KEY (IdAnimal) REFERENCES Animal(Id) ON DELETE CASCADE,
    CONSTRAINT FK_MedicalRecord_Veterinarian FOREIGN KEY (IdVeterinarian) REFERENCES Veterinarian(Id) ON DELETE CASCADE
);
