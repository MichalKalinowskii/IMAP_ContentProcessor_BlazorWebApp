CREATE TABLE IF NOT EXISTS Token (
    ID INT AUTO_INCREMENT PRIMARY KEY, 
    emailGeneratedID INT, 
    email VARCHAR(255), 
    access_token VARCHAR(2048), 
    refresh_token VARCHAR(2048)
);

CREATE TABLE IF NOT EXISTS Mail (
    ID INT AUTO_INCREMENT PRIMARY KEY,
    TokenID INT,
    content TEXT,
    eml LONGBLOB,
    FOREIGN KEY (TokenID) REFERENCES Token(ID)
);


CREATE TABLE IF NOT EXISTS OrderDetail (
    ID INT AUTO_INCREMENT PRIMARY KEY,
    MailID INT,
    productName VARCHAR(2048),
    productQuantity INT,
    price DECIMAL,
    FOREIGN KEY (MailID) REFERENCES Mail(ID)
);