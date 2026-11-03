//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'estimate_revision_summary_response.g.dart';

/// EstimateRevisionSummaryResponse
///
/// Properties:
/// * [id] 
/// * [revisionNo] 
/// * [status] 
/// * [currency] 
/// * [netCost] 
/// * [netBeforeTax] 
/// * [grandTotal] 
/// * [marginRate] 
/// * [createdAtUtc] 
/// * [updatedAtUtc] 
@BuiltValue()
abstract class EstimateRevisionSummaryResponse implements Built<EstimateRevisionSummaryResponse, EstimateRevisionSummaryResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String get id;

  @BuiltValueField(wireName: r'revisionNo')
  int get revisionNo;

  @BuiltValueField(wireName: r'status')
  String get status;

  @BuiltValueField(wireName: r'currency')
  String get currency;

  @BuiltValueField(wireName: r'netCost')
  double get netCost;

  @BuiltValueField(wireName: r'netBeforeTax')
  double get netBeforeTax;

  @BuiltValueField(wireName: r'grandTotal')
  double get grandTotal;

  @BuiltValueField(wireName: r'marginRate')
  double get marginRate;

  @BuiltValueField(wireName: r'createdAtUtc')
  DateTime get createdAtUtc;

  @BuiltValueField(wireName: r'updatedAtUtc')
  DateTime get updatedAtUtc;

  EstimateRevisionSummaryResponse._();

  factory EstimateRevisionSummaryResponse([void updates(EstimateRevisionSummaryResponseBuilder b)]) = _$EstimateRevisionSummaryResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(EstimateRevisionSummaryResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<EstimateRevisionSummaryResponse> get serializer => _$EstimateRevisionSummaryResponseSerializer();
}

class _$EstimateRevisionSummaryResponseSerializer implements PrimitiveSerializer<EstimateRevisionSummaryResponse> {
  @override
  final Iterable<Type> types = const [EstimateRevisionSummaryResponse, _$EstimateRevisionSummaryResponse];

  @override
  final String wireName = r'EstimateRevisionSummaryResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    EstimateRevisionSummaryResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    yield r'id';
    yield serializers.serialize(
      object.id,
      specifiedType: const FullType(String),
    );
    yield r'revisionNo';
    yield serializers.serialize(
      object.revisionNo,
      specifiedType: const FullType(int),
    );
    yield r'status';
    yield serializers.serialize(
      object.status,
      specifiedType: const FullType(String),
    );
    yield r'currency';
    yield serializers.serialize(
      object.currency,
      specifiedType: const FullType(String),
    );
    yield r'netCost';
    yield serializers.serialize(
      object.netCost,
      specifiedType: const FullType(double),
    );
    yield r'netBeforeTax';
    yield serializers.serialize(
      object.netBeforeTax,
      specifiedType: const FullType(double),
    );
    yield r'grandTotal';
    yield serializers.serialize(
      object.grandTotal,
      specifiedType: const FullType(double),
    );
    yield r'marginRate';
    yield serializers.serialize(
      object.marginRate,
      specifiedType: const FullType(double),
    );
    yield r'createdAtUtc';
    yield serializers.serialize(
      object.createdAtUtc,
      specifiedType: const FullType(DateTime),
    );
    yield r'updatedAtUtc';
    yield serializers.serialize(
      object.updatedAtUtc,
      specifiedType: const FullType(DateTime),
    );
  }

  @override
  Object serialize(
    Serializers serializers,
    EstimateRevisionSummaryResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required EstimateRevisionSummaryResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'id':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType(String),
          ) as String;
          result.id = valueDes;
          break;
        case r'revisionNo':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType(int),
          ) as int;
          result.revisionNo = valueDes;
          break;
        case r'status':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType(String),
          ) as String;
          result.status = valueDes;
          break;
        case r'currency':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType(String),
          ) as String;
          result.currency = valueDes;
          break;
        case r'netCost':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType(double),
          ) as double;
          result.netCost = valueDes;
          break;
        case r'netBeforeTax':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType(double),
          ) as double;
          result.netBeforeTax = valueDes;
          break;
        case r'grandTotal':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType(double),
          ) as double;
          result.grandTotal = valueDes;
          break;
        case r'marginRate':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType(double),
          ) as double;
          result.marginRate = valueDes;
          break;
        case r'createdAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType(DateTime),
          ) as DateTime;
          result.createdAtUtc = valueDes;
          break;
        case r'updatedAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType(DateTime),
          ) as DateTime;
          result.updatedAtUtc = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  EstimateRevisionSummaryResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = EstimateRevisionSummaryResponseBuilder();
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


