---
status: accepted
---

# Clean Architecture สี่ Project

Backend ใช้ Domain, Application, Infrastructure และ Api รวมสี่ Project แล้วแบ่ง Business Feature เป็น Folder ภายใน แทนการสร้างหลาย Project ต่อ Module หรือใช้ Controller–Service–Generic Repository เพราะรูปแบบที่เลือกคง Dependency Direction ชัด ขณะเดียวกันจำนวน Project และ Abstraction ยังดูแลง่ายสำหรับทีมขนาดเล็กถึงกลาง
