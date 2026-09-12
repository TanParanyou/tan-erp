"use client";

import React from "react";
import { Modal } from "@/components/ui/Modal";
import { Button } from "@/components/ui/Button";

export interface IosInstallModalProps {
  isOpen: boolean;
  onClose: () => void;
}

export function IosInstallModal({ isOpen, onClose }: IosInstallModalProps) {
  return (
    <Modal isOpen={isOpen} onClose={onClose} title="ติดตั้งแอปพลิเคชันบน iOS" size="sm">
      <div className="space-y-3.5 text-xs text-erp-text-main text-left">
        <p className="text-erp-text-muted leading-relaxed">
          สามารถติดตั้ง Project ERP ลงบนหน้าจอโฮมของ iPhone / iPad เพื่อการใช้งานที่รวดเร็วเสมือนแอปพื้นฐาน:
        </p>
        <ol className="list-decimal pl-4 space-y-2 leading-relaxed">
          <li>แตะปุ่ม <strong>แชร์ (Share)</strong> ที่แถบด้านล่างของ Safari</li>
          <li>เลื่อนลงและเลือก <strong>"เพิ่มไปยังหน้าจอโฮม" (Add to Home Screen)</strong></li>
          <li>แตะ <strong>"เพิ่ม" (Add)</strong> ที่มุมขวาบนเพื่อยืนยัน</li>
        </ol>
        <div className="pt-3 border-t border-erp-border flex justify-end">
          <Button variant="primary" size="sm" onClick={onClose}>
            เข้าใจแล้ว
          </Button>
        </div>
      </div>
    </Modal>
  );
}

IosInstallModal.displayName = "IosInstallModal";
