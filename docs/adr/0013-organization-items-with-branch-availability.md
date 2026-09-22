---
status: accepted
---

# Item เป็นข้อมูลระดับองค์กรและเปิดใช้ผ่าน Branch Availability

Item หนึ่งรายการมีตัวตนเดียวภายใน Organization และใช้ `availability_mode=all_branches|selected_branches` ร่วมกับ `item_branch_availabilities` เพื่อกำหนดสาขาที่เลือกใช้ แทนการคัดลอก Item ต่อสาขาหรือใส่ `branch_id` บน Item; ราคาประจำสาขายังคงเป็น Branch-scoped Cost Record แยกจาก Availability แนวทางนี้ป้องกันรหัส/ชื่อ/รูปภาพแตกเป็นหลายสำเนาและรองรับต้นทุนต่างสาขา แลกกับการที่ทุก Catalog Query ต้องรับ Branch Context และบังคับ Same-organization Relationship
