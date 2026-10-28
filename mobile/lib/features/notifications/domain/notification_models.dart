import 'package:flutter/foundation.dart';

@immutable
class FieldNotificationItem {
  const FieldNotificationItem({
    required this.id,
    required this.type,
    required this.payload,
    required this.createdAtUtc,
    this.deepLink,
    this.readAtUtc,
  });

  factory FieldNotificationItem.fromJson(Map<String, Object?> json) {
    final rawPayload = json['payload'];
    final parsedPayload = <String, String>{};
    if (rawPayload is Map<String, Object?>) {
      rawPayload.forEach((key, value) {
        if (value != null) {
          parsedPayload[key] = value.toString();
        }
      });
    }

    final rawCreated = json['createdAtUtc'] as String?;
    final createdAt = rawCreated != null
        ? DateTime.tryParse(rawCreated)?.toUtc() ?? DateTime.now().toUtc()
        : DateTime.now().toUtc();

    final rawRead = json['readAtUtc'] as String?;
    final readAt = rawRead != null ? DateTime.tryParse(rawRead)?.toUtc() : null;

    return FieldNotificationItem(
      id: json['id'] as String? ?? '',
      type: json['type'] as String? ?? '',
      payload: parsedPayload,
      deepLink: json['deepLink'] as String?,
      createdAtUtc: createdAt,
      readAtUtc: readAt,
    );
  }

  final String id;
  final String type;
  final Map<String, String> payload;
  final String? deepLink;
  final DateTime createdAtUtc;
  final DateTime? readAtUtc;

  bool get isUnread => readAtUtc == null;

  String get displayBody {
    final parts = <String>[];
    final docNo =
        payload['documentNumber'] ??
        payload['surveyNumber'] ??
        payload['estimateNumber'] ??
        payload['poNumber'] ??
        payload['code'];
    final title =
        payload['title'] ??
        payload['name'] ??
        payload['projectName'] ??
        payload['summary'];
    final msg = payload['message'] ?? payload['reason'] ?? payload['status'];

    if (docNo != null && docNo.isNotEmpty) parts.add(docNo);
    if (title != null && title.isNotEmpty) parts.add(title);
    if (msg != null && msg.isNotEmpty) parts.add(msg);

    if (parts.isEmpty && payload.isNotEmpty) {
      for (final entry in payload.entries) {
        if (entry.value.isNotEmpty) {
          parts.add('${entry.key}: ${entry.value}');
        }
      }
    }
    return parts.join(' · ');
  }

  FieldNotificationItem copyWith({
    DateTime? Function()? readAtUtc,
  }) {
    return FieldNotificationItem(
      id: id,
      type: type,
      payload: payload,
      deepLink: deepLink,
      createdAtUtc: createdAtUtc,
      readAtUtc: readAtUtc != null ? readAtUtc() : this.readAtUtc,
    );
  }

  Map<String, Object?> toJson() => <String, Object?>{
    'id': id,
    'type': type,
    'payload': payload,
    'deepLink': deepLink,
    'createdAtUtc': createdAtUtc.toIso8601String(),
    'readAtUtc': readAtUtc?.toIso8601String(),
  };
}
