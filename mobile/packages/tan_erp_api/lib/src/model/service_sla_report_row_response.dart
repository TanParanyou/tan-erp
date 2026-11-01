//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'service_sla_report_row_response.g.dart';

/// ServiceSlaReportRowResponse
///
/// Properties:
/// * [serviceRequestId] 
/// * [number] 
/// * [title] 
/// * [projectCode] 
/// * [branchCode] 
/// * [priority] 
/// * [status] 
/// * [inWarranty] 
/// * [slaStatus] 
/// * [elapsedHours] 
/// * [resolveTargetHours] 
/// * [serviceChargeAmount] 
/// * [createdAtUtc] 
@BuiltValue()
abstract class ServiceSlaReportRowResponse implements Built<ServiceSlaReportRowResponse, ServiceSlaReportRowResponseBuilder> {
  @BuiltValueField(wireName: r'serviceRequestId')
  String? get serviceRequestId;

  @BuiltValueField(wireName: r'number')
  String? get number;

  @BuiltValueField(wireName: r'title')
  String? get title;

  @BuiltValueField(wireName: r'projectCode')
  String? get projectCode;

  @BuiltValueField(wireName: r'branchCode')
  String? get branchCode;

  @BuiltValueField(wireName: r'priority')
  String? get priority;

  @BuiltValueField(wireName: r'status')
  String? get status;

  @BuiltValueField(wireName: r'inWarranty')
  bool? get inWarranty;

  @BuiltValueField(wireName: r'slaStatus')
  String? get slaStatus;

  @BuiltValueField(wireName: r'elapsedHours')
  double? get elapsedHours;

  @BuiltValueField(wireName: r'resolveTargetHours')
  int? get resolveTargetHours;

  @BuiltValueField(wireName: r'serviceChargeAmount')
  double? get serviceChargeAmount;

  @BuiltValueField(wireName: r'createdAtUtc')
  DateTime? get createdAtUtc;

  ServiceSlaReportRowResponse._();

  factory ServiceSlaReportRowResponse([void updates(ServiceSlaReportRowResponseBuilder b)]) = _$ServiceSlaReportRowResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(ServiceSlaReportRowResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<ServiceSlaReportRowResponse> get serializer => _$ServiceSlaReportRowResponseSerializer();
}

class _$ServiceSlaReportRowResponseSerializer implements PrimitiveSerializer<ServiceSlaReportRowResponse> {
  @override
  final Iterable<Type> types = const [ServiceSlaReportRowResponse, _$ServiceSlaReportRowResponse];

  @override
  final String wireName = r'ServiceSlaReportRowResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    ServiceSlaReportRowResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.serviceRequestId != null) {
      yield r'serviceRequestId';
      yield serializers.serialize(
        object.serviceRequestId,
        specifiedType: const FullType(String),
      );
    }
    if (object.number != null) {
      yield r'number';
      yield serializers.serialize(
        object.number,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.title != null) {
      yield r'title';
      yield serializers.serialize(
        object.title,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.projectCode != null) {
      yield r'projectCode';
      yield serializers.serialize(
        object.projectCode,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.branchCode != null) {
      yield r'branchCode';
      yield serializers.serialize(
        object.branchCode,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.priority != null) {
      yield r'priority';
      yield serializers.serialize(
        object.priority,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.status != null) {
      yield r'status';
      yield serializers.serialize(
        object.status,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.inWarranty != null) {
      yield r'inWarranty';
      yield serializers.serialize(
        object.inWarranty,
        specifiedType: const FullType(bool),
      );
    }
    if (object.slaStatus != null) {
      yield r'slaStatus';
      yield serializers.serialize(
        object.slaStatus,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.elapsedHours != null) {
      yield r'elapsedHours';
      yield serializers.serialize(
        object.elapsedHours,
        specifiedType: const FullType(double),
      );
    }
    if (object.resolveTargetHours != null) {
      yield r'resolveTargetHours';
      yield serializers.serialize(
        object.resolveTargetHours,
        specifiedType: const FullType(int),
      );
    }
    if (object.serviceChargeAmount != null) {
      yield r'serviceChargeAmount';
      yield serializers.serialize(
        object.serviceChargeAmount,
        specifiedType: const FullType(double),
      );
    }
    if (object.createdAtUtc != null) {
      yield r'createdAtUtc';
      yield serializers.serialize(
        object.createdAtUtc,
        specifiedType: const FullType(DateTime),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    ServiceSlaReportRowResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required ServiceSlaReportRowResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'serviceRequestId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.serviceRequestId = valueDes;
          break;
        case r'number':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.number = valueDes;
          break;
        case r'title':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.title = valueDes;
          break;
        case r'projectCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.projectCode = valueDes;
          break;
        case r'branchCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.branchCode = valueDes;
          break;
        case r'priority':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.priority = valueDes;
          break;
        case r'status':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.status = valueDes;
          break;
        case r'inWarranty':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(bool),
          ) as bool?;
          if (valueDes == null) continue;
          result.inWarranty = valueDes;
          break;
        case r'slaStatus':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.slaStatus = valueDes;
          break;
        case r'elapsedHours':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.elapsedHours = valueDes;
          break;
        case r'resolveTargetHours':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.resolveTargetHours = valueDes;
          break;
        case r'serviceChargeAmount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.serviceChargeAmount = valueDes;
          break;
        case r'createdAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.createdAtUtc = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  ServiceSlaReportRowResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = ServiceSlaReportRowResponseBuilder();
    final serializedList = (serialized as Iterable<Object?>).toList();
    final unhandled = <Object?>[];
    _deserializeProperties(
      serializers,
      serialized,
      specifiedType: specifiedType,
      serializedList: serializedList,
      unhandled: unhandled,
      result: result,
    );
    return result.build();
  }
}


