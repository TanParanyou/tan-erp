//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'create_calculation_policy_request.g.dart';

/// CreateCalculationPolicyRequest
///
/// Properties:
/// * [branchId] 
/// * [policyCode] 
/// * [version] 
/// * [overheadMethod] 
/// * [overheadValue] 
/// * [roundingMode] 
/// * [unitConversionMode] 
/// * [effectiveFromUtc] 
/// * [effectiveToUtc] 
@BuiltValue()
abstract class CreateCalculationPolicyRequest implements Built<CreateCalculationPolicyRequest, CreateCalculationPolicyRequestBuilder> {
  @BuiltValueField(wireName: r'branchId')
  String? get branchId;

  @BuiltValueField(wireName: r'policyCode')
  String get policyCode;

  @BuiltValueField(wireName: r'version')
  int? get version;

  @BuiltValueField(wireName: r'overheadMethod')
  String get overheadMethod;

  @BuiltValueField(wireName: r'overheadValue')
  double? get overheadValue;

  @BuiltValueField(wireName: r'roundingMode')
  String get roundingMode;

  @BuiltValueField(wireName: r'unitConversionMode')
  String get unitConversionMode;

  @BuiltValueField(wireName: r'effectiveFromUtc')
  DateTime? get effectiveFromUtc;

  @BuiltValueField(wireName: r'effectiveToUtc')
  DateTime? get effectiveToUtc;

  CreateCalculationPolicyRequest._();

  factory CreateCalculationPolicyRequest([void updates(CreateCalculationPolicyRequestBuilder b)]) = _$CreateCalculationPolicyRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(CreateCalculationPolicyRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<CreateCalculationPolicyRequest> get serializer => _$CreateCalculationPolicyRequestSerializer();
}

class _$CreateCalculationPolicyRequestSerializer implements PrimitiveSerializer<CreateCalculationPolicyRequest> {
  @override
  final Iterable<Type> types = const [CreateCalculationPolicyRequest, _$CreateCalculationPolicyRequest];

  @override
  final String wireName = r'CreateCalculationPolicyRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    CreateCalculationPolicyRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.branchId != null) {
      yield r'branchId';
      yield serializers.serialize(
        object.branchId,
        specifiedType: const FullType.nullable(String),
      );
    }
    yield r'policyCode';
    yield serializers.serialize(
      object.policyCode,
      specifiedType: const FullType(String),
    );
    if (object.version != null) {
      yield r'version';
      yield serializers.serialize(
        object.version,
        specifiedType: const FullType(int),
      );
    }
    yield r'overheadMethod';
    yield serializers.serialize(
      object.overheadMethod,
      specifiedType: const FullType(String),
    );
    if (object.overheadValue != null) {
      yield r'overheadValue';
      yield serializers.serialize(
        object.overheadValue,
        specifiedType: const FullType(double),
      );
    }
    yield r'roundingMode';
    yield serializers.serialize(
      object.roundingMode,
      specifiedType: const FullType(String),
    );
    yield r'unitConversionMode';
    yield serializers.serialize(
      object.unitConversionMode,
      specifiedType: const FullType(String),
    );
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
  }

  @override
  Object serialize(
    Serializers serializers,
    CreateCalculationPolicyRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required CreateCalculationPolicyRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
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
            specifiedType: const FullType(String),
          ) as String;
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
        case r'overheadMethod':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType(String),
          ) as String;
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
            specifiedType: const FullType(String),
          ) as String;
          result.roundingMode = valueDes;
          break;
        case r'unitConversionMode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType(String),
          ) as String;
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
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  CreateCalculationPolicyRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = CreateCalculationPolicyRequestBuilder();
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


