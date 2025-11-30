UPDATE Application SET Status = 'На рассмотрении' WHERE Status = 'Pending';
UPDATE Application SET Status = 'Одобрена' WHERE Status = 'Approved';
UPDATE Application SET Status = 'Отклонена' WHERE Status = 'Rejected';
