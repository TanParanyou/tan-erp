//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/qe_person_response.dart';
import 'package:tan_erp_api/src/model/date.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'branch_rate_override_response.g.dart';

/// BranchRateOverrideResponse
///
/// Properties:
/// * [id] 
/// * [templateId] 
/// * [branchId] 
/// * [branchCode] 
/// * [status] 
/// * [referenceRate] 
/// * [minimumCharge] 
/// * [effectiveFrom] 
/// * [effectiveTo] 
/// * [reasonCode] 
/// * [reasonDetail] 
/// * [sourceReference] 
/// * [requestedBy] 
/// * [requestedAtUtc] 
/// * [decidedBy] 
/// * [decidedAtUtc] 
/// * [decisionNote] 
/// * [rowVersion] 
@BuiltValue()
abstract class BranchRateOverrideResponse implements Built<BranchRateOverrideResponse, BranchRateOverrideResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'templateId')
  String? get templateId;

  @BuiltValueField(wireName: r'branchId')
  String? get branchId;

  @BuiltValueField(wireName: r'branchCode')
  String? get branchCode;

  @BuiltValueField(wireName: r'status')
  String? get status;

  @BuiltValueField(wireName: r'referenceRate')
  double? get referenceRate;

  @BuiltValueField(wireName: r'minimumCharge')
  double? get minimumCharge;

  @BuiltValueField(wireName: r'effectiveFrom')
  Date? get effectiveFrom;

  @BuiltValueField(wireName: r'effectiveTo')
  Date? get effectiveTo;

  @BuiltValueField(wireName: r'reasonCode')
  String? get reasonCode;

  @BuiltValueField(wireName: r'reasonDetail')
  String? get reasonDetail;

  @BuiltValueField(wireName: r'sourceReference')
  String? get sourceReference;

  @BuiltValueField(wireName: r'requestedBy')
  QePersonResponse? get requestedBy;

  @BuiltValueField(wireName: r'requestedAtUtc')
  DateTime? get requestedAtUtc;

  @BuiltValueField(wireName: r'decidedBy')
  QePersonResponse? get decidedBy;

  @BuiltValueField(wireName: r'decidedAtUtc')
  DateTime? get decidedAtUtc;

  @BuiltValueField(wireName: r'decisionNote')
  String? get decisionNote;

  @BuiltValueField(wireName: r'rowVersion')
  String? get rowVersion;

  BranchRateOverrideResponse._();

  factory BranchRateOverrideResponse([void updates(BranchRateOverrideResponseBuilder b)]) = _$BranchRateOverrideResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(BranchRateOverrideResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<BranchRateOverrideResponse> get serializer => _$BranchRateOverrideResponseSerializer();
}

class _$BranchRateOverrideResponseSerializer implements PrimitiveSerializer<BranchRateOverrideResponse> {
  @override
  final Iterable<Type> types = const [BranchRateOverrideResponse, _$BranchRateOverrideResponse];

  @override
  final String wireName = r'BranchRateOverrideResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    BranchRateOverrideResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.templateId != null) {
      yield r'templateId';
      yield serializers.serialize(
        object.templateId,
        specifiedType: const FullType(String),
      );
    }
    if (object.branchId != null) {
      yield r'branchId';
      yield serializers.serialize(
        object.branchId,
        specifiedType: const FullType(String),
      );
    }
    if (object.branchCode != null) {
      yield r'branchCode';
      yield serializers.serialize(
        object.branchCode,
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
    if (object.referenceRate != null) {
      yield r'referenceRate';
      yield serializers.serialize(
        object.referenceRate,
        specifiedType: const FullType(double),
      );
    }
    if (object.minimumCharge != null) {
      yield r'minimumCharge';
      yield serializers.serialize(
        object.minimumCharge,
        specifiedType: const FullType(double),
      );
    }
    if (object.effectiveFrom != null) {
      yield r'effectiveFrom';
      yield serializers.serialize(
        object.effectiveFrom,
        specifiedType: const FullType(Date),
      );
    }
    if (object.effectiveTo != null) {
      yield r'effectiveTo';
      yield serializers.serialize(
        object.effectiveTo,
        specifiedType: const FullType(Date),
      );
    }
    if (object.reasonCode != null) {
      yield r'reasonCode';
      yield serializers.serialize(
        object.reasonCode,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.reasonDetail != null) {
      yield r'reasonDetail';
      yield serializers.serialize(
        object.reasonDetail,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.sourceReference != null) {
      yield r'sourceReference';
      yield serializers.serialize(
        object.sourceReference,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.requestedBy != null) {
      yield r'requestedBy';
      yield serializers.serialize(
        object.requestedBy,
        specifiedType: const FullType(QePersonResponse),
      );
    }
    if (object.requestedAtUtc != null) {
      yield r'requestedAtUtc';
      yield serializers.serialize(
        object.requestedAtUtc,
        specifiedType: const FullType(DateTime),
      );
    }
    if (object.decidedBy != null) {
      yield r'decidedBy';
      yield serializers.serialize(
        object.decidedBy,
        specifiedType: const FullType(QePersonResponse),
      );
    }
    if (object.decidedAtUtc != null) {
      yield r'decidedAtUtc';
      yield serializers.serialize(
        object.decidedAtUtc,
        specifiedType: const FullType.nullable(DateTime),
      );
    }
    if (object.decisionNote != null) {
      yield r'decisionNote';
      yield serializers.serialize(
        object.decisionNote,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.rowVersion != null) {
      yield r'rowVersion';
      yield serializers.serialize(
        object.rowVersion,
        specifiedType: const FullType(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    BranchRateOverrideResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required BranchRateOverrideResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'id':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.id = valueDes;
          break;
        case r'templateId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.templateId = valueDes;
          break;
        case r'branchId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.branchId = valueDes;
          break;
        case r'branchCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.branchCode = valueDes;
          break;
        case r'status':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.status = valueDes;
          break;
        case r'referenceRate':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.referenceRate = valueDes;
          break;
        case r'minimumCharge':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.minimumCharge = valueDes;
          break;
        case r'effectiveFrom':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(Date),
          ) as Date?;
          if (valueDes == null) continue;
          result.effectiveFrom = valueDes;
          break;
        case r'effectiveTo':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(Date),
          ) as Date?;
          if (valueDes == null) continue;
          result.effectiveTo = valueDes;
          break;
        case r'reasonCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.reasonCode = valueDes;
          break;
        case r'reasonDetail':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.reasonDetail = valueDes;
          break;
        case r'sourceReference':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.sourceReference = valueDes;
          break;
        case r'requestedBy':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(QePersonResponse),
          ) as QePersonResponse?;
          if (valueDes == null) continue;
          result.requestedBy.replace(valueDes);
          break;
        case r'requestedAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.requestedAtUtc = valueDes;
          break;
        case r'decidedBy':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(QePersonResponse),
          ) as QePersonResponse?;
          if (valueDes == null) continue;
          result.decidedBy.replace(valueDes);
          break;
        case r'decidedAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.decidedAtUtc = valueDes;
          break;
        case r'decisionNote':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.decisionNote = valueDes;
          break;
        case r'rowVersion':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.rowVersion = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  BranchRateOverrideResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = BranchRateOverrideResponseBuilder();
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


