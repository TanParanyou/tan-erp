//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'update_cost_record_request.g.dart';

/// UpdateCostRecordRequest
///
/// Properties:
/// * [amount] 
/// * [currency] 
/// * [unitId] 
/// * [minimumQuantity] 
/// * [maximumQuantity] 
/// * [effectiveFromUtc] 
/// * [effectiveToUtc] 
/// * [costSourceId] 
/// * [sourceReference] 
/// * [reason] 
/// * [evidenceFileId] 
@BuiltValue()
abstract class UpdateCostRecordRequest implements Built<UpdateCostRecordRequest, UpdateCostRecordRequestBuilder> {
  @BuiltValueField(wireName: r'amount')
  double get amount;

  @BuiltValueField(wireName: r'currency')
  String get currency;

  @BuiltValueField(wireName: r'unitId')
  String get unitId;

  @BuiltValueField(wireName: r'minimumQuantity')
  double? get minimumQuantity;

  @BuiltValueField(wireName: r'maximumQuantity')
  double? get maximumQuantity;

  @BuiltValueField(wireName: r'effectiveFromUtc')
  DateTime get effectiveFromUtc;

  @BuiltValueField(wireName: r'effectiveToUtc')
  DateTime? get effectiveToUtc;

  @BuiltValueField(wireName: r'costSourceId')
  String? get costSourceId;

  @BuiltValueField(wireName: r'sourceReference')
  String? get sourceReference;

  @BuiltValueField(wireName: r'reason')
  String? get reason;

  @BuiltValueField(wireName: r'evidenceFileId')
  String? get evidenceFileId;

  UpdateCostRecordRequest._();

  factory UpdateCostRecordRequest([void updates(UpdateCostRecordRequestBuilder b)]) = _$UpdateCostRecordRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(UpdateCostRecordRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<UpdateCostRecordRequest> get serializer => _$UpdateCostRecordRequestSerializer();
}

class _$UpdateCostRecordRequestSerializer implements PrimitiveSerializer<UpdateCostRecordRequest> {
  @override
  final Iterable<Type> types = const [UpdateCostRecordRequest, _$UpdateCostRecordRequest];

  @override
  final String wireName = r'UpdateCostRecordRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    UpdateCostRecordRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    yield r'amount';
    yield serializers.serialize(
      object.amount,
      specifiedType: const FullType(double),
    );
    yield r'currency';
    yield serializers.serialize(
      object.currency,
      specifiedType: const FullType(String),
    );
    yield r'unitId';
    yield serializers.serialize(
      object.unitId,
      specifiedType: const FullType(String),
    );
    if (object.minimumQuantity != null) {
      yield r'minimumQuantity';
      yield serializers.serialize(
        object.minimumQuantity,
        specifiedType: const FullType(double),
      );
    }
    if (object.maximumQuantity != null) {
      yield r'maximumQuantity';
      yield serializers.serialize(
        object.maximumQuantity,
        specifiedType: const FullType.nullable(double),
      );
    }
    yield r'effectiveFromUtc';
    yield serializers.serialize(
      object.effectiveFromUtc,
      specifiedType: const FullType(DateTime),
    );
    if (object.effectiveToUtc != null) {
      yield r'effectiveToUtc';
      yield serializers.serialize(
        object.effectiveToUtc,
        specifiedType: const FullType.nullable(DateTime),
      );
    }
    if (object.costSourceId != null) {
      yield r'costSourceId';
      yield serializers.serialize(
        object.costSourceId,
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
    if (object.reason != null) {
      yield r'reason';
      yield serializers.serialize(
        object.reason,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.evidenceFileId != null) {
      yield r'evidenceFileId';
      yield serializers.serialize(
        object.evidenceFileId,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    UpdateCostRecordRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required UpdateCostRecordRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'amount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType(double),
          ) as double;
          result.amount = valueDes;
          break;
        case r'currency':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType(String),
          ) as String;
          result.currency = valueDes;
          break;
        case r'unitId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType(String),
          ) as String;
          result.unitId = valueDes;
          break;
        case r'minimumQuantity':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.minimumQuantity = valueDes;
          break;
        case r'maximumQuantity':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.maximumQuantity = valueDes;
          break;
        case r'effectiveFromUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType(DateTime),
          ) as DateTime;
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
        case r'costSourceId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.costSourceId = valueDes;
          break;
        case r'sourceReference':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.sourceReference = valueDes;
          break;
        case r'reason':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.reason = valueDes;
          break;
        case r'evidenceFileId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.evidenceFileId = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  UpdateCostRecordRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = UpdateCostRecordRequestBuilder();
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


