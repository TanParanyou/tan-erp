//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'catalog_resolved_cost_response.g.dart';

/// CatalogResolvedCostResponse
///
/// Properties:
/// * [costRecordId] 
/// * [version] 
/// * [amount] 
/// * [currency] 
/// * [unitCode] 
/// * [scope] 
/// * [effectiveFromUtc] 
/// * [policyVersion] 
/// * [costSourceId] 
/// * [costSourceCode] 
/// * [sourceReference] 
/// * [evidenceFileId] 
@BuiltValue()
abstract class CatalogResolvedCostResponse implements Built<CatalogResolvedCostResponse, CatalogResolvedCostResponseBuilder> {
  @BuiltValueField(wireName: r'costRecordId')
  String? get costRecordId;

  @BuiltValueField(wireName: r'version')
  int? get version;

  @BuiltValueField(wireName: r'amount')
  double? get amount;

  @BuiltValueField(wireName: r'currency')
  String? get currency;

  @BuiltValueField(wireName: r'unitCode')
  String? get unitCode;

  @BuiltValueField(wireName: r'scope')
  String? get scope;

  @BuiltValueField(wireName: r'effectiveFromUtc')
  DateTime? get effectiveFromUtc;

  @BuiltValueField(wireName: r'policyVersion')
  String? get policyVersion;

  @BuiltValueField(wireName: r'costSourceId')
  String? get costSourceId;

  @BuiltValueField(wireName: r'costSourceCode')
  String? get costSourceCode;

  @BuiltValueField(wireName: r'sourceReference')
  String? get sourceReference;

  @BuiltValueField(wireName: r'evidenceFileId')
  String? get evidenceFileId;

  CatalogResolvedCostResponse._();

  factory CatalogResolvedCostResponse([void updates(CatalogResolvedCostResponseBuilder b)]) = _$CatalogResolvedCostResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(CatalogResolvedCostResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<CatalogResolvedCostResponse> get serializer => _$CatalogResolvedCostResponseSerializer();
}

class _$CatalogResolvedCostResponseSerializer implements PrimitiveSerializer<CatalogResolvedCostResponse> {
  @override
  final Iterable<Type> types = const [CatalogResolvedCostResponse, _$CatalogResolvedCostResponse];

  @override
  final String wireName = r'CatalogResolvedCostResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    CatalogResolvedCostResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.costRecordId != null) {
      yield r'costRecordId';
      yield serializers.serialize(
        object.costRecordId,
        specifiedType: const FullType(String),
      );
    }
    if (object.version != null) {
      yield r'version';
      yield serializers.serialize(
        object.version,
        specifiedType: const FullType(int),
      );
    }
    if (object.amount != null) {
      yield r'amount';
      yield serializers.serialize(
        object.amount,
        specifiedType: const FullType(double),
      );
    }
    if (object.currency != null) {
      yield r'currency';
      yield serializers.serialize(
        object.currency,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.unitCode != null) {
      yield r'unitCode';
      yield serializers.serialize(
        object.unitCode,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.scope != null) {
      yield r'scope';
      yield serializers.serialize(
        object.scope,
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
    if (object.policyVersion != null) {
      yield r'policyVersion';
      yield serializers.serialize(
        object.policyVersion,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.costSourceId != null) {
      yield r'costSourceId';
      yield serializers.serialize(
        object.costSourceId,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.costSourceCode != null) {
      yield r'costSourceCode';
      yield serializers.serialize(
        object.costSourceCode,
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
    CatalogResolvedCostResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required CatalogResolvedCostResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'costRecordId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.costRecordId = valueDes;
          break;
        case r'version':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.version = valueDes;
          break;
        case r'amount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.amount = valueDes;
          break;
        case r'currency':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.currency = valueDes;
          break;
        case r'unitCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.unitCode = valueDes;
          break;
        case r'scope':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.scope = valueDes;
          break;
        case r'effectiveFromUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.effectiveFromUtc = valueDes;
          break;
        case r'policyVersion':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.policyVersion = valueDes;
          break;
        case r'costSourceId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.costSourceId = valueDes;
          break;
        case r'costSourceCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.costSourceCode = valueDes;
          break;
        case r'sourceReference':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.sourceReference = valueDes;
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
  CatalogResolvedCostResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = CatalogResolvedCostResponseBuilder();
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


