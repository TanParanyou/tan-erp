//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'calculation_policy_version_response.g.dart';

/// CalculationPolicyVersionResponse
///
/// Properties:
/// * [id] 
/// * [organizationId] 
/// * [branchId] 
/// * [policyCode] 
/// * [version] 
/// * [status] 
/// * [overheadMethod] 
/// * [overheadValue] 
/// * [roundingMode] 
/// * [unitConversionMode] 
/// * [effectiveFromUtc] 
/// * [effectiveToUtc] 
/// * [createdByUserId] 
/// * [publishedByUserId] 
/// * [publishedAtUtc] 
/// * [contentHash] 
@BuiltValue()
abstract class CalculationPolicyVersionResponse implements Built<CalculationPolicyVersionResponse, CalculationPolicyVersionResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'organizationId')
  String? get organizationId;

  @BuiltValueField(wireName: r'branchId')
  String? get branchId;

  @BuiltValueField(wireName: r'policyCode')
  String? get policyCode;

  @BuiltValueField(wireName: r'version')
  int? get version;

  @BuiltValueField(wireName: r'status')
  String? get status;

  @BuiltValueField(wireName: r'overheadMethod')
  String? get overheadMethod;

  @BuiltValueField(wireName: r'overheadValue')
  double? get overheadValue;

  @BuiltValueField(wireName: r'roundingMode')
  String? get roundingMode;

  @BuiltValueField(wireName: r'unitConversionMode')
  String? get unitConversionMode;

  @BuiltValueField(wireName: r'effectiveFromUtc')
  DateTime? get effectiveFromUtc;

  @BuiltValueField(wireName: r'effectiveToUtc')
  DateTime? get effectiveToUtc;

  @BuiltValueField(wireName: r'createdByUserId')
  String? get createdByUserId;

  @BuiltValueField(wireName: r'publishedByUserId')
  String? get publishedByUserId;

  @BuiltValueField(wireName: r'publishedAtUtc')
  DateTime? get publishedAtUtc;

  @BuiltValueField(wireName: r'contentHash')
  String? get contentHash;

  CalculationPolicyVersionResponse._();

  factory CalculationPolicyVersionResponse([void updates(CalculationPolicyVersionResponseBuilder b)]) = _$CalculationPolicyVersionResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(CalculationPolicyVersionResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<CalculationPolicyVersionResponse> get serializer => _$CalculationPolicyVersionResponseSerializer();
}

class _$CalculationPolicyVersionResponseSerializer implements PrimitiveSerializer<CalculationPolicyVersionResponse> {
  @override
  final Iterable<Type> types = const [CalculationPolicyVersionResponse, _$CalculationPolicyVersionResponse];

  @override
  final String wireName = r'CalculationPolicyVersionResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    CalculationPolicyVersionResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.organizationId != null) {
      yield r'organizationId';
      yield serializers.serialize(
        object.organizationId,
        specifiedType: const FullType(String),
      );
    }
    if (object.branchId != null) {
      yield r'branchId';
      yield serializers.serialize(
        object.branchId,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.policyCode != null) {
      yield r'policyCode';
      yield serializers.serialize(
        object.policyCode,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.version != null) {
      yield r'version';
      yield serializers.serialize(
        object.version,
        specifiedType: const FullType(int),
      );
    }
    if (object.status != null) {
      yield r'status';
      yield serializers.serialize(
        object.status,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.overheadMethod != null) {
      yield r'overheadMethod';
      yield serializers.serialize(
        object.overheadMethod,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.overheadValue != null) {
      yield r'overheadValue';
      yield serializers.serialize(
        object.overheadValue,
        specifiedType: const FullType(double),
      );
    }
    if (object.roundingMode != null) {
      yield r'roundingMode';
      yield serializers.serialize(
        object.roundingMode,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.unitConversionMode != null) {
      yield r'unitConversionMode';
      yield serializers.serialize(
        object.unitConversionMode,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.effectiveFromUtc != null) {
      yield r'effectiveFromUtc';
      yield serializers.serialize(
        object.effectiveFromUtc,
        specifiedType: const FullType(DateTime),
      );
    }
    if (object.effectiveToUtc != null) {
      yield r'effectiveToUtc';
      yield serializers.serialize(
        object.effectiveToUtc,
        specifiedType: const FullType.nullable(DateTime),
      );
    }
    if (object.createdByUserId != null) {
      yield r'createdByUserId';
      yield serializers.serialize(
        object.createdByUserId,
        specifiedType: const FullType(String),
      );
    }
    if (object.publishedByUserId != null) {
      yield r'publishedByUserId';
      yield serializers.serialize(
        object.publishedByUserId,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.publishedAtUtc != null) {
      yield r'publishedAtUtc';
      yield serializers.serialize(
        object.publishedAtUtc,
        specifiedType: const FullType.nullable(DateTime),
      );
    }
    if (object.contentHash != null) {
      yield r'contentHash';
      yield serializers.serialize(
        object.contentHash,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    CalculationPolicyVersionResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required CalculationPolicyVersionResponseBuilder result,
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
        case r'organizationId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.organizationId = valueDes;
          break;
        case r'branchId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.branchId = valueDes;
          break;
        case r'policyCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.policyCode = valueDes;
          break;
        case r'version':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.version = valueDes;
          break;
        case r'status':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.status = valueDes;
          break;
        case r'overheadMethod':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.overheadMethod = valueDes;
          break;
        case r'overheadValue':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.overheadValue = valueDes;
          break;
        case r'roundingMode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.roundingMode = valueDes;
          break;
        case r'unitConversionMode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.unitConversionMode = valueDes;
          break;
        case r'effectiveFromUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.effectiveFromUtc = valueDes;
          break;
        case r'effectiveToUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.effectiveToUtc = valueDes;
          break;
        case r'createdByUserId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.createdByUserId = valueDes;
          break;
        case r'publishedByUserId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.publishedByUserId = valueDes;
          break;
        case r'publishedAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.publishedAtUtc = valueDes;
          break;
        case r'contentHash':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.contentHash = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  CalculationPolicyVersionResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = CalculationPolicyVersionResponseBuilder();
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


